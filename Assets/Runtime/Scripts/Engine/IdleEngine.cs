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
using BreakInfinity;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public partial class IdleEngine : ScriptingContext, IEventSource
    {
        private System.Random random;
        public bool IsReady { get; private set; }
        public Dictionary<string, object> GlobalProperties = new Dictionary<string, object>();
        private Dictionary<string, string> propertyCalculators = new Dictionary<string, string>();
        private List<Timer> timers = new List<Timer>();

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
            if (Entities.ContainsKey(entity.Id))
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
                    (entity.ToObject() as IEventSource).Emit(EngineReadyEvent.EventName, Tuple.Create<string, object>("engine", this));
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
         * Create a timer that, after time seconds, evaluated the specified handler as a script.
         */
        public void Schedule(double time, string handler)
        {
            timers.Add(new Timer(time, handler));
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

        public void Emit(string eventName, Tuple<string, object> context)
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
                Scripting.EvaluateStringAsScript(handlerScript);
            }
        }

        /*
         * Whenever an event of the specified type is received at the given path, run the script in listener. This subscription is identified by description.
         */
        public void Watch(string eventName, string subscriber, DynValue handler)
        {
            listeners.Watch(eventName, subscriber, handler);
        }

        // Random Number
        public BigDouble RandomInt(int count)
        {
            return new BigDouble(random.Next(count));
        }

        public object PickRandom(ICollection options)
        {
            var index = random.Next(options.Count);
            int i = 0;
            foreach(var option in options)
            {
                if(i == index)
                {
                    return option;
                }
                i++;
            }
            return null;
        }

        private Dictionary<Type, List<Tuple<FieldInfo, PropertyInfo>>> typeTraversableFields = new Dictionary<Type, List<Tuple<FieldInfo, PropertyInfo>>>();

        public IEnumerable<DynValue> TraverseObjectGraph()
        {
            Queue<DynValue> queue = new Queue<DynValue>();
            foreach (var global in GlobalProperties)
            {
                DynValue globalValue = DynValue.FromObject(null, global.Value);
                if (globalValue.Type == DataType.Table || globalValue.Type == DataType.Tuple || (globalValue.Type == DataType.UserData && globalValue.ToObject() is ITraversableType))
                {
                    queue.Enqueue(globalValue);
                }
            }
            while (queue.Count > 0)
            {
                DynValue next = queue.Dequeue();
                IEnumerable children = null;
                switch (next.Type)
                {
                    case DataType.Table:
                        children = next.Table.Values;
                        break;
                    case DataType.Tuple:
                        children = next.Tuple;
                        break;
                    case DataType.UserData:
                        var asObject = next.ToObject();
                        if (asObject is ITraversableType)
                        {
                            children = (asObject as ITraversableType).GetTraversableFields();
                        }
                        break;
                }
                if(children != null)
                {
                    foreach (var child in children)
                    {
                        var value = DynValue.FromObject(null, child);
                        if (value.Type == DataType.UserData || value.Type == DataType.Table || value.Type == DataType.Tuple)
                        {
                            queue.Enqueue(value);
                        }
                    }
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
                            var updateable = nextToUpdate.ToObject() as IUpdateable;
                            if (updateable != null)
                            {
                                updateable.Update(this, deltaTime);
                            }
                            break;
                    }
                }
                foreach (var calculator in propertyCalculators)
                {
                    GlobalProperties[calculator.Key] = scripting.EvaluateStringAsScript(calculator.Value, new Dictionary<string, object>()
                    {
                        { "value", GlobalProperties.ContainsKey(calculator.Key) ? GlobalProperties[calculator.Key] : null },
                        { "deltaTime", deltaTime }
                    }).ToObject();
                }

                foreach (var timer in timers.ToArray())
                {
                    timer.Update(this, deltaTime);
                    if (timer.Triggered)
                    {
                        timers.Remove(timer);
                    }
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
            newModule.SetConfiguration(this);
            newModule.SetDefinitions(this);
            newModule.SetGlobalProperties(this);
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