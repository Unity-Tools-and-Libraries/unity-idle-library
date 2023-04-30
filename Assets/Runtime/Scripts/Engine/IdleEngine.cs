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
using io.github.thisisnozaku.scripting.context;
using io.github.thisisnozaku.logging;
using io.github.thisisnozaku.idle.framework.Engine.Achievements;
using io.github.thisisnozaku.idle.framework.Engine.Achievements.Events;
using Newtonsoft.Json.Serialization;
using UnityEngine.AddressableAssets;
using System.Diagnostics;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class IdleEngine : IScriptingContext
    {
        private System.Random random;
        public bool IsReady { get; private set; }
        private Dictionary<string, object> Configuration = new Dictionary<string, object>();
        private Dictionary<string, object> Definitions = new Dictionary<string, object>();
        public PropertiesHolder GlobalProperties = new PropertiesHolder();
        private Dictionary<string, string> propertyCalculators = new Dictionary<string, string>();
        private long nextTimerId = 1;
        private Dictionary<long, Timer> timers = new Dictionary<long, Timer>();

        private readonly EventListeners listeners;

        private LoggingModule logging;
        private ScriptingService scripting;
        // Services
        public ScriptingService Scripting => scripting;
        public LoggingModule Logging => logging;
        public Dictionary<double, Entity> Entities = new Dictionary<double, Entity>();

        //public StateMachine State { get; }

        [JsonIgnore]
        public AchievementsModule Achievements { get; }

        public void OverrideRandomNumberGenerator(System.Random rng)
        {
            random = rng;
        }

        public IdleEngine()
        {
            random = new System.Random();
            scripting = new ScriptingService(this);
            logging = new LoggingModule();
            listeners = new EventListeners(this);
            GlobalProperties["definitions"] = new Dictionary<string, object>();
            Achievements = new AchievementsModule();

            //State = new StateMachine(this);

            Logging.Log("Creating idle engine instance.");

            SerializationSettings = new JsonSerializerSettings();
            SerializationSettings.TypeNameHandling = TypeNameHandling.All;
            SerializationSettings.Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.All, this);
            SerializationSettings.TraceWriter = new LoggerTraceWriter(logging);
            SerializationSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
        }

        public void RegisterEntity(Entity entity)
        {
            Logging.Log(string.Format("Registering entity with id {0}", entity.Id), "entity");
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

        public JsonSerializerSettings SerializationSettings;

        public void DeserializeSnapshotString(string snapshot)
        {
            Logging.Log("Deserializing from snapshot string", "persistence");

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
        public void Schedule(double time, string handler, string description = "", bool repeat = false)
        {
            Logging.Log("Creating new scheduled task", "timers");
            timers.Add(nextTimerId++, new Timer(time, handler, description, repeat));
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
            string[] tokens = GeneratePathTokens(path);
            startingFrom = startingFrom == null ? GlobalProperties : startingFrom;
            object currentObject;
            if (!startingFrom.TryGetValue(tokens[0], out currentObject))
            {
                return null;
            }
            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i];
                object key = token;
                if (typeof(IDictionary).IsAssignableFrom(currentObject.GetType()))
                {
                    var genericArgs = currentObject.GetType().GetGenericArguments();

                    if (genericArgs[0] == typeof(long))
                    {
                        key = long.Parse((string)key);
                    }
                    else if (genericArgs[0] == typeof(int))
                    {
                        key = int.Parse((string)key);
                    }
                    else if (genericArgs[0] == typeof(double))
                    {
                        key = double.Parse((string)key);
                    }
                    else if (genericArgs[0] == typeof(float))
                    {
                        key = float.Parse((string)key);
                    }
                    currentObject = (currentObject as IDictionary)[key];
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

                return null;
            }
            return currentObject;
        }

        public V GetDefinition<V>(string path)
        {
            return GetProperty<V>(path, Definitions);
        }

        public void SetConfiguration(string property, object value)
        {
            Configuration[property] = value;
        }

        public IDictionary<string, object> GetConfiguration()
        {
            return Configuration;
        }

        public object GetConfiguration(string path)
        {
            return GetProperty(path, GetConfiguration());
        }

        public T GetConfiguration<T>(string path)
        {
            return GetProperty<T>(path, GetConfiguration());
        }

        public object GetExpectedConfiguration(string path)
        {
            object value = GetProperty(path, GetConfiguration());
            if (value == null)
            {
                throw new InvalidOperationException(String.Format("Failed to find a value for mandatory configuration at {0}", path));
            }
            return value;
        }
        /**
         * As GetConfiguration<T>, except that if no value is found it throws.
         * 
         * Use this when your configuration must have a value so that you will learn that it is missing as soon as possible, instead of 
         * debugging problems caused by unexpected nulls.
         */
        public T GetExpectedConfiguration<T>(string path)
        {
            T value = GetProperty<T>(path, GetConfiguration());
            if (value == null)
            {
                throw new InvalidOperationException(String.Format("Failed to find a value for mandatory configuration at {0}", path));
            }
            return value;
        }

        public IDictionary<string, object> GetDefinitions()
        {
            return Definitions;
        }

        /*
         * Notify all listeners watching the global scope that the given event has occurred.
         */
        public void Emit(string eventName, IScriptingContext context)
        {
            Logging.Log(string.Format("Emitting global event {0}", eventName), "events");
            listeners.Emit(eventName, context != null ? context.GetContextVariables() : null);
        }

        /*
         * Notify all listeners watching the global scope that the given event has occurred.
         */
        public void Emit(string eventName, IDictionary<string, object> context)
        {
            Logging.Log(string.Format("Emitting global event {0}", eventName), "events");
            listeners.Emit(eventName, context);
        }

        public void Emit(string eventName, Tuple<string, object> context)
        {
            Logging.Log(string.Format("Emitting global event {0}", eventName), "events");
            listeners.Emit(eventName, context);
        }

        /*
         * Whenever an event of the specified type is received at the given path, run the script in listener. This subscription is identified by description.
         */
        public void Watch(string eventName, string subscriber, string handlerScript)
        {
            listeners.Watch(eventName, subscriber, handlerScript);
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
            foreach (var option in options)
            {
                if (i == index)
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
                if (global.Key != "configuration" && global.Key != "definitions")
                {
                    DynValue globalValue = DynValue.FromObject(null, global.Value);
                    if (globalValue.Type == DataType.Table || globalValue.Type == DataType.Tuple || (globalValue.Type == DataType.UserData && globalValue.ToObject() is ITraversableType))
                    {
                        queue.Enqueue(globalValue);
                    }
                }
            }
            while (queue.Count > 0)
            {
                DynValue next = queue.Dequeue();
                IEnumerable<object> children = null;
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
                        if (asObject is Entity)
                        {
                            if (children == null)
                            {
                                children = (asObject as Entity).ExtraProperties.Values.AsEnumerable<object>();
                            }
                            else
                            {
                                children = Enumerable.Concat(children, (asObject as Entity).ExtraProperties.Values.AsEnumerable<object>());
                            }
                        }
                        break;
                }
                if (children != null)
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

        public double GetNextAvailableId()
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
                    timer.Value.Update(this, deltaTime);
                    if (timer.Value.Triggered && !timer.Value.Repeat)
                    {
                        timers.Remove(timer.Key);
                    }
                }

                foreach (var achievement in Achievements)
                {
                    if (!achievement.Value.Completed)
                    {
                        achievement.Value.Completed = scripting.Evaluate(achievement.Value.CompletionExpression).Boolean;
                        if (achievement.Value.Completed)
                        {
                            Emit(AchievementCompletedEvent.EventName, new AchievementCompletedEvent(achievement.Value));
                            if (achievement.Value.CompletionEffect != null)
                            {
                                Scripting.Evaluate(achievement.Value.CompletionEffect);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Don't call update until after calling Start");
            }
        }

        public Timer GetTimer(int id)
        {
            Timer timer;
            timers.TryGetValue(id, out timer);
            return timer;
        }

        public void RestoreFromSnapshot(EngineSnapshot snapshot)
        {
            Logging.Log("Restoring from snapshot object", "persitence");
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
            foreach (var achievement in snapshot.Achievements)
            {
                if (Achievements[achievement.Id] == null)
                {
                    throw new Exception();
                }
                Achievements[achievement.Id].Completed = achievement.Completed;
            }
            Emit(EngineReadyEvent.EventName, (IScriptingContext)null);
        }

        public void AddModule(IModule newModule, bool loadScripts = false)
        {
            if (IsReady)
            {
                throw new InvalidOperationException("Can only add modules before starting the engine.");
            }
            newModule.Configure(this);
        }
        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>();
        }

        private static HashSet<Type> serializationExcludedTypes = new HashSet<Type>()
        {
            typeof(Closure),
            typeof(Table)
        };

        public EngineSnapshot GetSnapshot()
        {
            var globals = new Dictionary<string, object>(GlobalProperties);
            return new EngineSnapshot(globals
                .Where(e => e.Key != "definitions")
                .ToDictionary(e => e.Key, e => e.Value),
                Achievements.Values.ToList(),
                listeners.GetListeners());
        }

        public T GetPlayer<T>() where T : Player
        {
            return GetPlayer() as T;
        }

        /**
         * Non-generic version of GetPlayer<T>, primarily for use in scripting.
         * 
         * Prefer the generic method in C# code.
         */
        public Player GetPlayer()
        {
            if (!GlobalProperties.ContainsKey("player"))
            {
                throw new InvalidOperationException("No player property defined.");
            }
            return GlobalProperties["player"] as Player;
        }

        public void StopWatching(string eventName, string subscriptionDescription)
        {
            listeners.StopWatching(eventName, subscriptionDescription);
        }

        private static Dictionary<string, WeakReference<string[]>> cachedPaths = new Dictionary<string, WeakReference<string[]>>();

        public static string[] GeneratePathTokens(string path)
        {
            string[] value = null;
            if (!cachedPaths.ContainsKey(path) || !cachedPaths[path].TryGetTarget(out value))
            {
                value = path.Split('.').SelectMany(t =>
            {
                if (t.Contains("["))
                {
                    return new string[]
                    {
                            t.Substring(0, t.IndexOf("[")),
                            t.Substring(t.IndexOf("[") + 1 , t.IndexOf("]") - t.IndexOf("[") - 1)
                    };
                }
                else
                {
                    return new string[] { t };
                }
            }).ToArray();
            }
            return value;
        }

        public void DefineAchievement(Achievement achievement)
        {
            Achievements[achievement.Id] = achievement;
        }

        private class LoggerTraceWriter : ITraceWriter
        {
            private LoggingModule logging;

            public LoggerTraceWriter(LoggingModule logging)
            {
                this.logging = logging;
            }
            public TraceLevel LevelFilter => TraceLevel.Verbose;

            public void Trace(TraceLevel level, string message, Exception ex)
            {
                logging.Log(() => message, "serialization");
            }
        }
    }
}