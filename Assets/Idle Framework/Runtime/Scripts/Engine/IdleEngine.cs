using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Scripting;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public partial class IdleEngine : RandomNumberSource, IDefinitionManager, ScriptingContext
    {
        private MonoBehaviour gameObject;
        private System.Random random;
        private static Regex PathRegex = new Regex("^[a-zA-Z_-][a-zA-Z0-9_-]*(\\.[a-zA-Z0-9_-]+)*$");
        private IDictionary<string, ValueContainer> references = new Dictionary<string, ValueContainer>();
        private Dictionary<string, Func<ScriptingContext>> ScriptingContextResolvers = new Dictionary<string, Func<ScriptingContext>>();
        public bool IsReady { get; private set; }
        private readonly IDictionary<string, ValueContainer> globalProperties = new Dictionary<string, ValueContainer>();
        private readonly EventListeners listeners;
        private DefinitionManager definitions = new DefinitionManager();
        private ScriptingService scripting;
        private Queue<Tuple<string, string, IDictionary<string, object>, object[]>> NotificationQueue = new Queue<Tuple<string, string, IDictionary<string, object>, object[]>>();
        public ScriptingService Scripting => scripting;

        public static void ValidatePath(string path)
        {
            if (path != null && !PathRegex.Match(path).Success)
            {
                throw new ArgumentException(path);
            }
        }

        public void OverrideRandomNumberGenerator(System.Random rng)
        {
            random = rng;
        }

        public IdleEngine(MonoBehaviour gameObject)
        {
            this.gameObject = gameObject;
            random = new System.Random();
            scripting = new ScriptingService(this);
            listeners = new EventListeners(this);
            ScriptingContextResolvers["global"] = () => this;

            GlobalScriptingContext["getOrCreate"] = new CallbackFunction((ctx, args) =>
            {
                return DynValue.FromObject(ctx.GetScript(), GetProperty(args[0].CastToString(), GetOperationType.GET_OR_CREATE));
            });
        }

        public void Start()
        {
            Log(LogType.Log, () => "Starting engine", "engine.internal");
            IsReady = true;
            Broadcast(EngineReadyEvent.EventName, null);
        }

        // Event Management
        public void Broadcast(string eventName, string eventSource, ScriptingContext context = null)
        {
            Queue<ValueContainer> broadcastTargets = new Queue<ValueContainer>();
            Log(LogType.Log, () => "Broadcasting " + eventName, "engine.internal.events");
            if (eventSource == null)
            {
                foreach (var global in globalProperties.Values)
                {
                    broadcastTargets.Enqueue(global);
                }
            }
            else
            {
                var targetProperty = GetProperty(eventSource);
                if (targetProperty != null)
                {
                    broadcastTargets.Enqueue(targetProperty);
                }
            }
            listeners.Notify(eventName, null, this);
            ValueContainer next;
            while (broadcastTargets.Count() > 0)
            {
                next = broadcastTargets.Dequeue();
                listeners.Notify(eventName, next.Path, next);
                if (eventName == EngineReadyEvent.EventName)
                {
                    next.NotifyImmediately(eventName, context);
                }
                if (next.DataType == "map")
                {
                    foreach (var child in next.ValueAsMap().Values)
                    {
                        broadcastTargets.Enqueue(child);
                    }
                }
                else if (next.DataType == "list")
                {
                    foreach (var child in next.ValueAsList())
                    {
                        broadcastTargets.Enqueue(child);
                    }
                }
            }
        }
        public void NotifyImmediately(string eventName)
        {
            NotifyImmediately(eventName, null, this);
        }
        public void NotifyImmediately(string eventName, string eventSource, ScriptingContext context)
        {
            listeners.Notify(eventName, eventSource, context);
            if (eventSource != null && eventName != ValueChangedEvent.EventName)
            {
                var sourceTokens = eventSource.Split('.');
                for (int i = sourceTokens.Length - 1; i > 0; i--)
                {
                    string targetPath = sourceTokens[0];
                    for (int t = 1; t < i; t++)
                    {
                        targetPath += "." + sourceTokens[t];
                    }
                    var targetContainer = GetProperty(targetPath, GetOperationType.GET_OR_THROW);
                    if (targetContainer != null && targetContainer.Path != eventSource)
                    {
                        listeners.Notify(eventName, targetContainer.Path, context);
                    }
                }
                listeners.Notify(eventName, null, context);
            }
        }

        public void NotifyImmediately(string eventName, string eventSource, string context)
        {
            NotifyImmediately(eventName, eventSource, ScriptingContextResolvers[context]());
        }

        internal void NotifyLater(string eventName, string eventTarget = null, IDictionary<string, object> context = null, params object[] args)
        {
            lock (NotificationQueue)
            {
                NotificationQueue.Enqueue(Tuple.Create(eventName, eventTarget, context, args));
            }
        }

        public void Unsubscribe(ListenerSubscription subscription)
        {
            listeners.Unsubscribe(subscription);
        }

        public ListenerSubscription Subscribe(string subscriber, string eventName, string listener, string targetName = null, bool ephemeral = false)
        {
            var subscription = listeners.Subscribe(targetName, subscriber, eventName, listener, ephemeral);
            if (eventName == EngineReadyEvent.EventName && IsReady)
            {
                EvaluateExpression(listener);
            }
            return subscription;
        }

        // Random Number
        public int RandomInt(int count)
        {
            return random.Next(count);
        }

        // Global Properties
        public ValueContainer GetProperty(string path, GetOperationType operationType = GetOperationType.GET_OR_NULL)
        {
            var tokens = path.Split('.');
            ValueContainer container = null;
            if (!globalProperties.TryGetValue(tokens[0], out container))
            {
                switch (operationType)
                {
                    case GetOperationType.GET_OR_NULL:
                        return null;
                    case GetOperationType.GET_OR_THROW:
                        throw new InvalidOperationException(String.Format("Tried to get a container @{0} but it doesn't exist.", tokens[0]));
                    case GetOperationType.GET_OR_CREATE:
                        container = globalProperties[tokens[0]] = CreateProperty(tokens[0]);
                        container.Path = tokens[0];
                        break;
                }
            }
            if (tokens.Length > 1)
            {
                return container.GetProperty(string.Join(".", tokens.Skip(1).ToArray()), operationType);
            }
            return container;
        }

        public enum GetOperationType
        {
            GET_OR_NULL,   // If a path reaches a non-existant container or an intermediate container which is not a dictionary, return null
            GET_OR_CREATE, // If a path reaches a non-existant container, create it; if it reaches a non-dictionary intermediate container, return null.
            GET_OR_THROW   // If a path reaches a non-existant container or an intermediate container which is not a dictionary, throw an exception
        }

        public ValueContainer CreateProperty(string property, ValueContainer value)
        {
            return DoReplaceProperty(property, value, globalProperties);
        }

        private ValueContainer DoReplaceProperty(string path, ValueContainer container, IDictionary<string, ValueContainer> properties, ValueContainer parent = null)
        {
            var pathTokens = path.Split('.');
            if (pathTokens.Length > 1)
            {
                var intermediateContainer = parent != null ? parent : GetProperty(pathTokens[0], GetOperationType.GET_OR_CREATE);
                if (intermediateContainer.ValueAsMap() == null)
                {
                    intermediateContainer.Set(new Dictionary<string, ValueContainer>());
                }
                DoReplaceProperty(string.Join(".", pathTokens.Skip(1)), container, intermediateContainer.AsMap, intermediateContainer);
            }
            else
            {
                ValueContainer existingContainer;
                string fullPath = parent != null ? string.Join(".", parent.Path, path) : path;
                if (properties.TryGetValue(path, out existingContainer))
                {
                    Log(LogType.Warning, string.Format("A container is being assigned to path '{0}', orphanining container #{1}", fullPath, existingContainer.Id), "engine.internal.containers");
                    existingContainer.Parent = null;
                    existingContainer.Path = null;
                }
                properties[path] = container;
                if (container != null)
                {
                    container.Path = fullPath;
                    container.Parent = parent;
                }
            }
            return container;
        }

        public ValueContainer CreateProperty(string property, bool value, string description = "", List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            return DoCreateProperty(property, value, description, modifiers, updater, interceptor);
        }

        public ValueContainer CreateProperty(string property, List<ValueContainer> value, string description = "", List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            return DoCreateProperty(property, value, description, modifiers, updater, interceptor);
        }

        public ValueContainer CreateProperty(string property, string value = null, string description = "", List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            return DoCreateProperty(property, value, description, modifiers, updater, interceptor);
        }

        public ValueContainer CreateProperty(string property, IDictionary<string, ValueContainer> value, string description = "", List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            return DoCreateProperty(property, value, description, modifiers, updater, interceptor);
        }

        public ValueContainer CreateProperty(string property, BigDouble value, string description = "", List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            return DoCreateProperty(property, value, description, modifiers, updater, interceptor);
        }

        private ValueContainer DoCreateProperty(string property, object value, string description, List<IContainerModifier> modifiers, string updater, string interceptor)
        {
            var existing = GetProperty(property);
            if (existing != null)
            {
                Log(LogType.Warning, string.Format("A new container is being assigned to path '{0}', orphanining container #{1}", property, existing.Id), "engine.internal.containers");
            }

            bool globalExists = false;
            var pathelements = property.Split('.').ToArray();
            ValueContainer parent = null;
            for (int i = 0; i < pathelements.Count() - 1; i++)
            {
                globalExists = true;
                var path = string.Join(".", pathelements.Take(i + 1));
                var newContainer = GetProperty(path, GetOperationType.GET_OR_CREATE);
                newContainer.Parent = parent;

                if (i == 0)
                {
                    GlobalScriptingContext[pathelements[0]] = newContainer;
                }
                if (newContainer.ValueAsMap() == null)
                {
                    newContainer.Set(new Dictionary<string, ValueContainer>());
                }
                parent = newContainer;
            }
            ValueContainer container = null;
            switch (DetermineType(value))
            {
                case "string":
                case "null":
                    container = CreateValueContainer(value as string, description, parent, modifiers, updater, interceptor);
                    break;
                case "number":
                    container = CreateValueContainer((BigDouble)value, description, parent, modifiers, updater, interceptor);
                    break;
                case "map":
                    container = CreateValueContainer(value as IDictionary<string, ValueContainer>, description, parent, modifiers, updater, interceptor);
                    break;
                case "list":
                    container = CreateValueContainer(value as IList<ValueContainer>, description, parent, modifiers, updater, interceptor);
                    break;
                case "bool":
                    container = CreateValueContainer((bool)value, description, parent, modifiers, updater, interceptor);
                    break;
            }
            if (parent != null)
            {
                parent.ValueAsMap()[pathelements[pathelements.Count() - 1]] = container;
            }
            if (!globalExists)
            {
                globalProperties[property] = container;
                GlobalScriptingContext[property] = container;

            }
            container.Path = property;
            return container;
        }

        public static string ReplacePlaceholders(string expression, Dictionary<string, string> values)
        {
            foreach (var contextVariable in values)
            {
                var toReplace = string.Format("${{{0}}}", contextVariable.Key);
                expression = expression.Replace(toReplace, contextVariable.Value);
            }
            return expression;
        }

        public void Update(float deltaTime)
        {
            if (IsReady)
            {
                var toIterate = new List<ValueContainer>(globalProperties.Values);
                foreach (var reference in toIterate)
                {
                    reference.Update(this, deltaTime);
                }
            }
        }

        private IEnumerator DispatchPendingNotifications()
        {
            while (true)
            {
                if (NotificationQueue.Count == 0)
                {
                    yield return null;
                }
                else
                {
                    double executionTime = 0;
                    while (executionTime < .01 && NotificationQueue.Count > 0)
                    {
                        double startTime = Time.realtimeSinceStartupAsDouble;
                        var next = NotificationQueue.Dequeue();
                        //NotifyImmediately(next.Item1, next.Item2, next.Item3, next.Item4);
                        executionTime += Time.realtimeSinceStartupAsDouble - startTime;
                    }
                    yield return null;
                }
            }
        }

        public ValueContainer CreateValueContainer(string value = null, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var id = NextId();
            references[id] = null;
            var vc = new ValueContainer(this, id, value as string, description, parent, modifiers, updater, interceptor);
            references[id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(BigDouble value, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var id = NextId();
            references[id] = null;
            var vc = new ValueContainer(this, id, value, description, parent, modifiers, updater, interceptor);
            references[id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(bool value, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var id = NextId();
            references[id] = null;
            var vc = new ValueContainer(this, id, value, description, parent, modifiers, updater, interceptor);
            references[id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(IDictionary<string, ValueContainer> value, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var id = NextId();
            references[id] = null;
            var vc = new ValueContainer(this, id, value, description, parent, modifiers, updater, interceptor);
            references[id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(IList<ValueContainer> value, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var id = NextId();
            references[id] = null;
            var vc = new ValueContainer(this, id, value, description, parent, modifiers, updater, interceptor);
            references[id] = vc;
            return vc;
        }

        private int evaluationDepth = 0;
        private int maxEvaluationDepth = 10;

        public object EvaluateExpression(string valueExpression, string contextToUse)
        {
            ScriptingContext resolvedContext = ScriptingContextResolvers[contextToUse]();
            return EvaluateExpression(valueExpression, resolvedContext.GetScriptingContext());
        }

        public object EvaluateExpression(string valueExpression, IDictionary<string, object> localContext = null)
        {
            evaluationDepth++;
            if(evaluationDepth > maxEvaluationDepth)
            {
                throw new InvalidOperationException(String.Format("Attempted to invoke too many evaluation operations (more than {0}). This may indicate an infinite loop!", maxEvaluationDepth));
            }
            if (valueExpression == null)
            {
                throw new ArgumentNullException("valueExpression");
            }

            var output = NormalizeValue(Scripting.DoString(valueExpression, localContext).ToObject());
            evaluationDepth--;
            return output;
        }

        private string NextId()
        {
            int count = references.Count();
            string nextId = null;
            while (nextId == null)
            {
                nextId = (count + 1).ToString();
                if (references.ContainsKey(nextId))
                {
                    nextId = null;
                    count++;
                }
            }
            return nextId;
        }

        public Snapshot GetSnapshot()
        {
            return new Snapshot(globalProperties.ToDictionary(x => x.Value.Path, x => x.Value.GetSnapshot()),
                listeners.listeners.SelectMany(x => x.Value.SelectMany(y => y.Value)).Where(l => !l.Ephemeral).ToList());
        }

        public void RestoreFromSnapshot(Snapshot snapshot)
        {
            foreach (var e in snapshot.GlobalProperties)
            {
                if (e.Value != null)
                {
                    GetProperty(e.Value.Path, GetOperationType.GET_OR_CREATE).RestoreFromSnapshot(this, e.Value);
                }
            }
            foreach (var l in snapshot.Listeners)
            {
                listeners.Subscribe(l.SubscriptionTarget, l.SubscriberDescription, l.EventName, l.MethodName, false);
            }
        }

        public class Snapshot
        {
            public readonly Dictionary<string, ValueContainer.Snapshot> GlobalProperties;
            public readonly List<ListenerSubscription> Listeners;

            public Snapshot(Dictionary<string, ValueContainer.Snapshot> globalProperties, List<ListenerSubscription> listeners)
            {
                GlobalProperties = globalProperties;
                this.Listeners = listeners;
            }
        }

        public void RegisterMethod(string name, Delegate method)
        {
            if (DynValue.FromObject(null, method).Type != DataType.ClrFunction)
            {
                throw new InvalidOperationException();
            };

            GlobalScriptingContext[name] = method;
        }

        public void RegisterMethod(string name, Func<ScriptExecutionContext, CallbackArguments, DynValue> method)
        {
            GlobalScriptingContext[name] = new CallbackFunction(method);
        }

        public T GetDefinition<T>(string typeName, string id) where T : IDefinition
        {
            return ((IDefinitionManager)definitions).GetDefinition<T>(typeName, id);
        }

        public ICollection<T> GetDefinitions<T>(string typeName) where T : IDefinition
        {
            return ((IDefinitionManager)definitions).GetDefinitions<T>(typeName);
        }

        public void SetDefinitions(string typeName, IDictionary<string, IDefinition> definitions)
        {
            ((IDefinitionManager)this.definitions).SetDefinitions(typeName, definitions);
        }

        private IDictionary<string, ValueContainer> ConvertProperties(IDictionary<string, object> properties)
        {
            return properties.ToDictionary(e => e.Key, e =>
            {
                if (Values.IsDictionary(e.Value))
                {
                    return CreateValueContainer(ConvertProperties(e.Value as IDictionary<string, object>));
                }
                else
                {
                    var value = ValueContainer.NormalizeValue(e.Value);
                    if (value is bool)
                    {
                        return CreateValueContainer((bool)value);
                    }
                    else if (Values.IsNumber(value))
                    {
                        return CreateValueContainer((BigDouble)value);
                    }
                    else if (value is string || value is null)
                    {
                        return CreateValueContainer((string)value);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            });
        }

        private Dictionary<string, Dictionary<LogType, bool>> LoggingContextLevels = new Dictionary<string, Dictionary<LogType, bool>>()
        {
            { "*", new Dictionary<LogType, bool>() { { LogType.Error, true} } }
        };

        public void Log(LogType logType, Func<string> logMessageGenerator, string logContext = null)
        {
            bool logEnabled = false;
            logContext = logContext != null ? logContext : "*";
            if (LoggingContextLevels.ContainsKey(logContext))
            {
                LoggingContextLevels[logContext].TryGetValue(logType, out logEnabled);
            }
            else
            {
                LoggingContextLevels["*"].TryGetValue(logType, out logEnabled);
            }
            if (logEnabled)
            {
                Log(logType, logMessageGenerator(), logContext);
            }
        }

        public void Log(LogType logType, string logMessage, string logContext = null)
        {
            bool logEnabled = false;
            logContext = logContext != null ? logContext : "*";
            if (LoggingContextLevels.ContainsKey(logContext))
            {
                LoggingContextLevels[logContext].TryGetValue(logType, out logEnabled);
            }
            else
            {
                LoggingContextLevels["*"].TryGetValue(logType, out logEnabled);
            }
            if (logEnabled)
            {
                string finalMessage = string.Format("[{0}] {1}", logContext, logMessage);
                switch (logType)
                {
                    case LogType.Error:
                    case LogType.Exception:
                        Debug.LogError(finalMessage);
                        break;
                    case LogType.Log:
                        Debug.Log(finalMessage);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(finalMessage);
                        break;
                }
            }
        }

        public void ConfigureLogging(string logContext, LogType? logLevel, bool enabled = true)
        {
            Dictionary<LogType, bool> contexts;
            switch (logLevel)
            {
                case LogType.Log:
                    ConfigureLogging(logContext, LogType.Warning, enabled);
                    break;
                case LogType.Warning:
                    ConfigureLogging(logContext, LogType.Error, enabled);
                    break;
            }
            if (!LoggingContextLevels.TryGetValue(logContext, out contexts))
            {
                contexts = new Dictionary<LogType, bool>();
                LoggingContextLevels[logContext] = contexts;
            }
            if (!logLevel.HasValue)
            {
                contexts.Clear();
            }
            else
            {
                contexts[logLevel.Value] = enabled;
            }
        }

        /*
         * Transform a definition into a dictionary containing the definition properties.
         */
        public ValueContainer InstantiateDefinitionInstance(IDefinition definition)
        {
            var properties = ConvertProperties(definition.Properties);
            return CreateValueContainer(value: properties);
        }

        public void AddModule(IModule newModule)
        {
            if (IsReady)
            {
                throw new InvalidOperationException("Can only add modules before starting the engine.");
            }
            newModule.AssertReady();
            foreach (var definition in newModule.GetDefinitions())
            {
                SetDefinitions(definition.Key, definition.Value);
            }
            newModule.ConfigureEngine(this);
        }

        private class EventListeners
        {
            public Dictionary<string, Dictionary<string, HashSet<ListenerSubscription>>> listeners = new Dictionary<string, Dictionary<string, HashSet<ListenerSubscription>>>();
            private IdleEngine engine;
            private int notificationCount = 0;
            public EventListeners(IdleEngine engine)
            {
                this.engine = engine;
            }

            internal void Notify(string eventName, string eventSource, ScriptingContext contextToUse)
            {
                notificationCount++;
                if (notificationCount > 100)
                {
                    throw new InvalidOperationException("Notification depth over 100! If you are performing so many notifications in the same frame, try using NotifyLater to schedule notifications to be done next frame.");
                }
                if (eventSource == null)
                {
                    eventSource = "";
                }
                Dictionary<string, HashSet<ListenerSubscription>> events;
                if (listeners.TryGetValue(eventSource, out events))
                {
                    HashSet<ListenerSubscription> eventListeners = null;
                    if (events.TryGetValue(eventName, out eventListeners))
                    {
                        var toIterate = eventListeners.ToArray();
                        foreach (var listener in toIterate)
                        {
                            try
                            {
                                engine.EvaluateExpression(listener.MethodName, contextToUse.GetScriptingContext());
                            }
                            catch (Exception ex)
                            {
                                engine.Log(LogType.Error, string.Format("Failed to invoke listner {0} for event {1} triggered from {2}: {3}", listener.MethodName, eventName, eventSource, ex));
                            }
                        }
                    }
                }
                notificationCount = 0;
            }

            internal ListenerSubscription Subscribe(string target, string subscriber, string eventName, string listenerName, bool ephemeral)
            {
                if (target == null)
                {
                    target = "";
                }
                ListenerSubscription subscription = new ListenerSubscription(target, subscriber, eventName, listenerName, ephemeral);
                Dictionary<string, HashSet<ListenerSubscription>> events;
                if (!listeners.TryGetValue(subscription.SubscriptionTarget, out events))
                {
                    events = new Dictionary<string, HashSet<ListenerSubscription>>();
                    listeners[subscription.SubscriptionTarget] = events;
                }
                HashSet<ListenerSubscription> eventListeners;
                if (!events.TryGetValue(subscription.EventName, out eventListeners))
                {
                    eventListeners = new HashSet<ListenerSubscription>();
                    events[subscription.EventName] = eventListeners;
                }
                eventListeners.Add(subscription);
                return subscription;
            }

            internal void Unsubscribe(ListenerSubscription subscription)
            {
                Dictionary<string, HashSet<ListenerSubscription>> events;
                if (listeners.TryGetValue(subscription.SubscriptionTarget, out events))
                {
                    HashSet<ListenerSubscription> eventListeners = null;
                    if (events.TryGetValue(subscription.EventName, out eventListeners))
                    {
                        eventListeners.Remove(subscription);
                    }
                }
            }
        }

        private Dictionary<string, object> GlobalScriptingContext = new Dictionary<string, object>()
        {
            { "math", new Dictionary<string, object>() {
                { "max", new CallbackFunction((ctx, args) => {
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Max(ScriptingService.DynValueToBigDouble(args[0]), ScriptingService.DynValueToBigDouble(args[1])));
                }) },
                { "min", new CallbackFunction((ctx, args) => {
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Min(ScriptingService.DynValueToBigDouble(args[0]), ScriptingService.DynValueToBigDouble(args[1])));
                }) },
                { "clamp", new CallbackFunction((ctx, args) => {
                    BigDouble value = ScriptingService.DynValueToBigDouble(args[0]);
                    BigDouble floor = ScriptingService.DynValueToBigDouble(args[1]);
                    BigDouble ceiling = ScriptingService.DynValueToBigDouble(args[2]);
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Max(floor, BigDouble.Min(ceiling, value)));
                }) }
            }}
        };
        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return GlobalScriptingContext;
        }
    }
}