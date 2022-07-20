using io.github.thisisnozaku.idle.framework.Engine.Logging;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using io.github.thisisnozaku.idle.framework.Engine.Scripting;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json.Serialization;
using BreakInfinity;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public partial class IdleEngine : ScriptingContext, IEventSource
    {
        private System.Random random;
        public bool IsReady { get; private set; }
        public Dictionary<string, object> GlobalProperties = new Dictionary<string, object>();
        private Dictionary<string, string> propertyCalculators = new Dictionary<string, string>();

        private readonly EventListeners listeners;

        private LoggingService logging;
        private ScriptingService scripting;
        // Services
        public ScriptingService Scripting => scripting;
        public LoggingService Logging => logging;
        public Dictionary<long, Entity> Entities = new Dictionary<long, Entity>();

        public void OverrideRandomNumberGenerator(System.Random rng)
        {
            random = rng;
        }

        public IdleEngine()
        {
            random = new System.Random();
            scripting = new ScriptingService(this);
            logging = new LoggingService();
            listeners = new EventListeners(this);
            GlobalProperties["configuration"] = new Dictionary<string, object>();
            GlobalProperties["definitions"] = new Dictionary<string, object>();

            SerializationSettings = new JsonSerializerSettings();
            SerializationSettings.TypeNameHandling = TypeNameHandling.All;
            SerializationSettings.Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.All, this);
        }

        public void RegisterEntity(Entity entity)
        {
            if(Entities.ContainsKey(entity.Id))
            {
                throw new InvalidOperationException(String.Format("Attempting to use entity id {0}, which is already in use.", entity.Id));
            }
            Entities[entity.Id] = entity;
        }

        public void Start()
        {
            logging.Log(LogType.Log, () => "Starting engine", "engine.internal");
            IsReady = true;
            Emit(EngineReadyEvent.EventName, this);
            foreach (var entity in TraverseObjectGraph())
            {
                if (entity.ToObject() is IEventSource)
                {
                    (entity.ToObject() as IEventSource).Emit(EngineReadyEvent.EventName);
                }
            }
        }

        public string GetSerializedSnapshotString()
        {
            return JsonConvert.SerializeObject(GetSnapshot(), SerializationSettings);
        }

        private JsonSerializerSettings SerializationSettings;

        public void DeserializeSnapshotString(string snapshot)
        {
            RestoreFromSnapshot(JsonConvert.DeserializeObject<EngineSnapshot>(snapshot, SerializationSettings));
        }

        public void CalculateProperty(string property, string calculationScript)
        {
            if (calculationScript == null)
            {
                propertyCalculators.Remove(property);
            }
            else
            {
                propertyCalculators[property] = calculationScript;
            }
        }

        

        /*
         * Convenience method to easily traverse the object graph of global properties.
         * 
         * Starting from the global properties, the object graph will be walked, returning null if not found.
         * 
         * Be aware, this method uses reflection and is thus slow.
         */
        public V GetProperty<V>(string path, IDictionary<string, object> startingFrom = null)
        {
            object found = GetProperty(path, startingFrom);

            return found != null && found is V ? (V)found : default(V);
        }

        public object GetProperty(string path, IDictionary<string, object> startingFrom = null)
        {
            string[] tokens = path.Split('.');
            startingFrom = startingFrom == null ? GlobalProperties : startingFrom;
            object currentObject;
            if (!startingFrom.TryGetValue(tokens[0], out currentObject))
            {
                return null;
            }
            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i];
                if (typeof(IDictionary).IsAssignableFrom(currentObject.GetType()))
                {
                    currentObject = (currentObject as IDictionary)[token];
                    continue;
                }

                FieldInfo field = currentObject.GetType().GetField(token);
                if (field != null)
                {
                    currentObject = field.GetValue(currentObject);
                    continue;
                }

                PropertyInfo property = currentObject.GetType().GetProperty(token);
                if (property != null)
                {
                    currentObject = property.GetValue(currentObject);
                    continue;
                }

                currentObject = null;
                continue;
            }
            return currentObject;
        }


        /*
         * Helper method for setting configuration values.
         * 
         * Sets values as children of the 'configuration' key in the global properties.
         */
        public void SetConfiguration(string property, object value)
        {
            (GlobalProperties["configuration"] as IDictionary<string, object>)[property] = value;
        }

        public IDictionary<string, object> GetConfiguration()
        {
            return GlobalProperties["configuration"] as IDictionary<string, object>;
        }

        public object GetConfiguration(string path)
        {
            return GetProperty(path, GetConfiguration());
        }

        public T GetConfiguration<T>(string path)
        {
            return GetProperty<T>(path, GetConfiguration());
        }

        public IDictionary<string, object> GetDefinitions()
        {
            return GlobalProperties["definitions"] as IDictionary<string, object>;
        }

        /*
         * Notify all listeners watching the global scope that the given event has occurred.
         */
        public void Emit(string eventName, ScriptingContext context)
        {
            listeners.Emit(eventName, context != null ? context.GetScriptingProperties() : null);
        }

        /*
         * Notify all listeners watching the global scope that the given event has occurred.
         */
        public void Emit(string eventName, IDictionary<string, object> context)
        {
            listeners.Emit(eventName, context);
        }

        /*
         * Whenever an event of the specified type is received at the given path, run the script in listener. This subscription is identified by description.
         */
        public void Watch(string eventName, string subscriber, string handlerScript)
        {
            listeners.Watch(eventName, subscriber, handlerScript);
            if (eventName == EngineReadyEvent.EventName && IsReady)
            {
                Scripting.EvaluateString(handlerScript);
            }
        }

        /*
         * Whenever an event of the specified type is received at the given path, run the script in listener. This subscription is identified by description.
         */
        public void Watch(string eventName, string subscriber, DynValue handler)
        {
            switch(handler.Type)
            {
                case DataType.String:
                    Watch(eventName, subscriber, handler.String);
                    break;
                case DataType.ClrFunction:
                    listeners.Watch(eventName, subscriber, handler.Callback);
                    if (eventName == EngineReadyEvent.EventName && IsReady)
                    {
                        Scripting.ExecuteCallback(handler.Callback);
                    }
                    break;
                default:
                    throw new ArgumentException("Handler must be a clr function or string wrapped in a DynValue.");
            }
        }

        // Random Number
        public BigDouble RandomInt(int count)
        {
            return new BigDouble(random.Next(count));
        }

        private Dictionary<Type, List<Tuple<FieldInfo, PropertyInfo>>> typeTraversableFields = new Dictionary<Type, List<Tuple<FieldInfo, PropertyInfo>>>();

        public IEnumerable<DynValue> TraverseObjectGraph()
        {
            Queue<object> queue = new Queue<object>();
            foreach (var global in GlobalProperties.Values)
            {
                var globalType = DynValue.FromObject(null, global).Type;
                if (globalType == DataType.UserData || globalType == DataType.Table || globalType == DataType.Tuple)
                {
                    queue.Enqueue(global);
                }
            }
            while (queue.Count > 0)
            {
                DynValue next = DynValue.FromObject(null, queue.Dequeue());
                switch (next.Type)
                {
                    case DataType.Table:
                        foreach (var child in next.Table.Values)
                        {
                            if (child.Type == DataType.UserData || child.Type == DataType.Table || child.Type == DataType.Tuple)
                            {
                                queue.Enqueue(child);
                            }
                        }
                        break;
                    case DataType.Tuple:
                        foreach (var item in next.Tuple)
                        {
                            if (item.Type == DataType.UserData || item.Type == DataType.Table || item.Type == DataType.Tuple)
                            {
                                queue.Enqueue(item);
                            }
                        }
                        break;
                    case DataType.UserData:
                        var asObject = next.ToObject();
                        if (asObject is ITraversableType)
                        {
                            foreach(var value in (asObject as ITraversableType).GetTraversableFields())
                            {
                                var item = DynValue.FromObject(null, value);
                                if (item.Type == DataType.UserData || item.Type == DataType.Table || item.Type == DataType.Tuple)
                                {
                                    queue.Enqueue(item);
                                }
                            }
                        }
                        break;

                }
                yield return next;
            }
        }

        public long GetNextAvailableId()
        {
            return Entities.Count > 0 ? Entities.Keys.Max() + 1 : 1;
        }

        public void Update(float deltaTime)
        {
            if (IsReady)
            {
                foreach (var nextToUpdate in TraverseObjectGraph())
                {
                    switch (nextToUpdate.Type)
                    {
                        case DataType.UserData:
                            var updateable = nextToUpdate.ToObject() as Entity;
                            if (updateable != null)
                            {
                                updateable.Update(this, deltaTime);
                            }
                            break;
                    }
                }
                foreach (var calculator in propertyCalculators)
                {
                    GlobalProperties[calculator.Key] = scripting.EvaluateString(calculator.Value, new Dictionary<string, object>()
                    {
                        { "value", GlobalProperties.ContainsKey(calculator.Key) ? GlobalProperties[calculator.Key] : null },
                        { "deltaTime", deltaTime }
                    }).ToObject();
                }
            }
            else
            {
                throw new InvalidOperationException("Don't call update until after calling Start");
            }
        }

        public void RestoreFromSnapshot(EngineSnapshot snapshot)
        {
            foreach (var e in snapshot.Properties)
            {
                if (e.Value != null)
                {
                    GlobalProperties[e.Key] = e.Value;
                }
            }
            if (snapshot.Listeners != null)
            {
                foreach (var @event in snapshot.Listeners)
                {
                    foreach (var subscriber in @event.Value)
                    {
                        listeners.Watch(@event.Key, subscriber.Key, subscriber.Value);
                    }
                }
            }
        }

        public void AddModule(IModule newModule)
        {
            if (IsReady)
            {
                throw new InvalidOperationException("Can only add modules before starting the engine.");
            }
            newModule.AssertReady();
            newModule.ConfigureEngine(this);
        }
        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>();
        }

        public EngineSnapshot GetSnapshot()
        {
            return new EngineSnapshot(GlobalProperties, listeners.GetListeners());
        }

        public void StopWatching(string eventName, string subscriptionDescription)
        {
            listeners.StopWatching(eventName, subscriptionDescription);
        }
    }
}