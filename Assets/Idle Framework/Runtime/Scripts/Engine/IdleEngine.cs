using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public partial class IdleEngine : RandomNumberSource, IDefinitionManager
    {
        private System.Random random;
        public delegate object UserMethod(IdleEngine engine, params object[] args);
        private static Regex PathRegex = new Regex("^[a-zA-Z_-][a-zA-Z0-9_-]*(\\.[a-zA-Z0-9_-]+)*$");
        private IDictionary<string, ValueContainer> references = new Dictionary<string, ValueContainer>();
        public bool IsReady { get; private set; }
        private readonly IDictionary<string, ValueContainer> globalProperties = new Dictionary<string, ValueContainer>();
        private readonly EventListeners listeners;
        private Dictionary<string, UserMethod> methods = new Dictionary<string, UserMethod>();
        private DefinitionManager definitions = new DefinitionManager();
        private ScriptingManagement scripting;

        public ScriptingManagement Scripting => scripting;

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

        public IdleEngine(GameObject gameObject)
        {
            random = new System.Random();
            scripting = new ScriptingManagement(this);
            listeners = new EventListeners(this);
        }

        public void Start()
        {
            Log(LogType.Log, "Starting engine", "engine.internal");
            IsReady = true;
            Broadcast(EngineReadyEvent.EventName, null);
        }

        // Event Management
        public void Broadcast(string eventName, string target, params object[] args)
        {
            Queue<ValueContainer> broadcastTargets = new Queue<ValueContainer>();
            Log(LogType.Log, "Broadcasting " + eventName, "engine.internal.events");
            if (target == null)
            {
                foreach (var global in globalProperties.Values)
                {
                    broadcastTargets.Enqueue(global);
                };
                NotifyImmediately(eventName, null, null, args);
            }
            else
            {
                var targetProperty = GetProperty(target);
                if (targetProperty != null)
                {
                    broadcastTargets.Enqueue(targetProperty);
                }
            }

            ValueContainer next;
            while(broadcastTargets.Count() > 0)
            {
                next = broadcastTargets.Dequeue();
                listeners.Notify(eventName, next.Path, args);
                if (next.DataType == "map")
                {
                    foreach (var child in next.ValueAsMap().Values) {
                        broadcastTargets.Enqueue(child);
                    }
                } else if (next.DataType == "list")
                {
                    foreach (var child in next.ValueAsList())
                    {
                        broadcastTargets.Enqueue(child);
                    }
                }
            }
        }

        public void NotifyImmediately(string eventName, string eventTarget = null, IDictionary<string, object> context = null, params object[] args)
        {
            listeners.Notify(eventName, eventTarget, context, args);
            if (eventTarget != null)
            {
                var targetContainer = GetProperty(eventTarget);
                if (targetContainer != null)
                {
                    BubbleEvent(eventName, targetContainer, args);
                }
            }
        }

        public void Unsubscribe(ListenerSubscription subscription)
        {
            listeners.Unsubscribe(subscription);
        }

        public void Unsubscribe(string subscriber, string eventName, string target = null)
        {
            listeners.Unsubscribe(subscriber, eventName, target);
        }

        public ListenerSubscription Subscribe(string subscriber, string eventName, string listenerName, string targetName = null, bool ephemeral = false)
        {
            return listeners.Subscribe(targetName, subscriber, eventName, listenerName, ephemeral);
        }

        public ListenerSubscription Subscribe(string subscriber, string eventName, UserMethod listener, string targetName = null, bool ephemeral = false)
        {
            return Subscribe(subscriber, eventName, listener.Method.Name, targetName, ephemeral);
        }

        internal void BubbleEvent(string eventName, ValueContainer valueContainer, params object[] args)
        {
            var sourcePath = valueContainer.Path;
            if (sourcePath != null)
            {
                var sourceTokens = sourcePath.Split('.');
                for (int i = sourceTokens.Length - 1; i > 0; i--)
                {
                    string targetPath = sourceTokens[0];
                    for (int t = 1; t < i; t++)
                    {
                        targetPath += "." + sourceTokens[t];
                    }
                    var targetContainer = GetProperty(targetPath, GetOperationType.GET_OR_THROW);
                    if (targetContainer != null && targetContainer != valueContainer)
                    {
                        targetContainer.DoNotification(eventName, args);
                    }
                }
            }
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
                        throw new InvalidOperationException();
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
            return DoCreateProperty(property, value.ValueAsRaw(), value.Description, new List<IContainerModifier>(value.GetModifiers()), value.GetUpdater(), value.GetInterceptor());
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
            bool globalExists = false;
            var pathelements = property.Split('.').ToArray();
            ValueContainer parent = null;
            for (int i = 0; i < pathelements.Count() - 1; i++)
            {
                globalExists = true;
                var path = string.Join(".", pathelements.Take(i + 1));
                var newContainer = GetProperty(path, GetOperationType.GET_OR_CREATE);
                newContainer.Parent = parent;
                newContainer.Path = path;
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

        private Func<ScriptExecutionContext, CallbackArguments, DynValue> WrapMethod(string methodName)
        {
            return (ctx, args) =>
            {
                var methodOut = NormalizeValue(methods[methodName].Invoke(this, args.GetArray().Select(arg => ValueContainer.NormalizeValue(arg.ToObject())).ToArray()));
                if (methodOut != null)
                {
                    if (methodOut is bool)
                    {
                        return DynValue.NewBoolean((bool)methodOut);
                    }
                    else if (methodOut is BigDouble || methodOut is string)
                    {
                        return DynValue.NewString(methodOut.ToString());
                    }
                    else if (Values.IsDictionary(methodOut.GetType()) || methodOut is ValueContainer)
                    {
                        return UserData.Create(methodOut);
                    }
                }
                return DynValue.Nil;
            };
        }

        public object EvaluateExpression(string valueExpression, IDictionary<string, object> localContext = null)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException("valueExpression");
            }

            return NormalizeValue(Scripting.DoString(valueExpression, localContext).ToObject());
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
            foreach(var l in snapshot.Listeners)
            {
                listeners.Subscribe(l.SubscriptionTarget, l.SubscriberDescription, l.Event, l.MethodName, false);
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

        public object InvokeMethod(string methodName, params object[] args)
        {
            UserMethod method;
            if (methods.TryGetValue(methodName, out method))
            {
                return method(this, args);
            }
            else
            {
                throw new InvalidOperationException("Couldn't find method " + methodName + ". Ensure that the method is registered before use.");
            }
        }

        public object InvokeMethod(UserMethod method, params object[] args)
        {
            return InvokeMethod(method.Method.Name, args);
        }

        public void RegisterMethod(UserMethod method)
        {
            RegisterMethod(method.Method.Name, method);
        }

        public void RegisterMethod(string name, UserMethod method)
        {
            if (methods.ContainsKey(name))
            {
                this.Log(LogType.Error, "The method " + name + " is being registered again.", "engine.internal");
            }
            methods[name] = method;
            //Scripting.Globals[name] = WrapMethod(name);
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
        /*
         * 
         */
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

        internal void DoNotification(string eventName, string target, object[] args)
        {
            listeners.Notify(eventName, target, args);
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
            foreach (var definition in newModule.GetDefinitions())
            {
                SetDefinitions(definition.Key, definition.Value);
            }
            newModule.ConfigureEngine(this);
        }

        internal IDictionary<string, object> GenerateGlobalContext()
        {
            var properties = new Dictionary<string, object>();
            foreach (var global in globalProperties)
            {
                properties[global.Key] = global.Value;
            }
            foreach (var method in methods)
            {
                properties[method.Key] = WrapMethod(method.Key);
            }
            return properties;
        }

        public class ScriptingManagement
        {
            internal Script script = new Script();
            public Table Globals => script.Globals;
            private IdleEngine engine;
            public ScriptingManagement(IdleEngine engine)
            {
                UserData.RegisterProxyType<ValueContainerScriptProxy, ValueContainer>(c => new ValueContainerScriptProxy(c));
                UserData.RegisterType<BigDouble>();
                SetScriptToClrCustomConversion(DataType.Number, typeof(BigDouble), (arg) =>
                {
                    return BigDouble.Parse(arg.CastToString());
                });
                SetScriptToClrCustomConversion(DataType.UserData, typeof(BigDouble), (arg) =>
                {
                    var obj = arg.ToObject();
                    if (obj is ValueContainer)
                    {
                        return (obj as ValueContainer).ValueAsNumber();
                    }
                    else if (obj is BigDouble)
                    {
                        return (BigDouble)obj;
                    }
                    else
                    {
                        return null;
                    }
                });
                SetScriptToClrCustomConversion(DataType.UserData, typeof(string), (arg) =>
                {
                    var obj = arg.ToObject();
                    if (obj is ValueContainer)
                    {
                        return (obj as ValueContainer).ValueAsString();
                    }
                    else
                    {
                        return obj.ToString();
                    }
                });
                SetScriptToClrCustomConversion(DataType.String, typeof(BigDouble), (arg) =>
                {
                    var converted = arg.CastToNumber();
                    if (converted != null)
                    {
                        return new BigDouble(converted.Value);
                    }
                    return BigDouble.NaN;
                });
                SetClrToScriptCustomConversion(typeof(BigDouble), (script, arg) =>
                {
                    return UserData.Create(arg);
                });
                SetClrToScriptCustomConversion(typeof(ValueContainer), (script, arg) =>
                {
                    return UserData.Create(arg);
                });
                this.engine = engine;
                GlobalIndexMethod = DynValue.NewCallback((ctx, args) =>
                {
                    string property = args[1].CastToString();
                    object found = this.engine.GetProperty(property, GetOperationType.GET_OR_NULL);
                    if (found != null)
                    {
                        return UserData.Create(found);
                    }
                    else
                    {
                        if (engine.methods.ContainsKey(property))
                        {
                            return DynValue.NewCallback((ctx, args) =>
                            {
                                object output = engine.methods[property].Invoke(engine, args.GetArray()
                                    .Select(x => ValueContainer.NormalizeValue(x.ToObject())).ToArray()); ;
                                if (output == null)
                                {
                                    return DynValue.Nil;
                                }
                                else
                                {
                                    return DynValue.FromObject(script, ValueContainer.NormalizeValue(output));
                                }
                            });
                        }
                        else if (ctx.CurrentGlobalEnv.Get(property) != DynValue.Nil)
                        {

                            return ctx.CurrentGlobalEnv.Get(property);
                        }
                        return DynValue.FromObject(script, engine.GetProperty(property, GetOperationType.GET_OR_CREATE));
                    }
                });
                script.Globals["getOrCreate"] = DynValue.NewCallback((ctx, args) =>
                {
                    var propertyObject = DoString("return " + args[0].CastToString(), ctx.CurrentGlobalEnv).ToObject();
                    string property;
                    if (propertyObject is ValueContainer)
                    {
                        property = (propertyObject as ValueContainer).Path;
                    }
                    else
                    {
                        property = propertyObject.ToString();
                    }

                    return DynValue.FromObject(ctx.GetScript(), engine.GetProperty(property, GetOperationType.GET_OR_CREATE));
                });
            }

            private DynValue GlobalIndexMethod;

            public Table GenerateContextTable(IDictionary<string, object> contextVariables = null)
            {
                var contextTable = new Table(script);
                foreach (var global in script.Globals.Pairs)
                {
                    contextTable.Set(global.Key, global.Value);
                }
                contextTable.MetaTable = new Table(script);
                contextTable.MetaTable.Set("__index", GlobalIndexMethod);
                if (contextVariables != null)
                {
                    foreach (var contextVariable in contextVariables)
                    {
                        contextTable.Set(contextVariable.Key, DynValue.FromObject(script, contextVariable.Value));
                    }
                }
                return contextTable;
            }

            public void SetClrToScriptCustomConversion(Type clrType, Func<object, object, DynValue> converter)
            {
                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(clrType, converter);
            }

            public void SetScriptToClrCustomConversion(DataType scriptDataType, Type clrDataType, Func<DynValue, object> converter)
            {
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(scriptDataType, clrDataType, converter);
            }

            public DynValue DoString(string expression, IDictionary<string, object> context = null)
            {
                return DoString(expression, GenerateContextTable(context));
            }

            public DynValue DoString(string expression, Table context = null)
            {
                return script.DoString(expression, context);
            }
        }

        private class EventListeners
        {
            public Dictionary<string, Dictionary<string, List<ListenerSubscription>>> listeners = new Dictionary<string, Dictionary<string, List<ListenerSubscription>>>();
            private IdleEngine engine;

            public EventListeners(IdleEngine engine)
            {
                this.engine = engine;
            }

            internal void Notify(string eventName, string eventTarget, IDictionary<string, object> context, object[] args)
            {
                if (eventTarget == null)
                {
                    eventTarget = "";
                }
                Dictionary<string, List<ListenerSubscription>> events;
                if (listeners.TryGetValue(eventTarget, out events))
                {
                    List<ListenerSubscription> eventListeners = null;
                    if (events.TryGetValue(eventName, out eventListeners))
                    {
                        {
                            foreach (var listener in eventListeners)
                            {
                                engine.InvokeMethod(listener.MethodName, args);
                            }
                        }
                    }
                }
            }

            internal void Notify(string eventName, string target, object[] args)
            {
                if (target == null)
                {
                    target = "";
                }
                Dictionary<string, List<ListenerSubscription>> events;
                if (listeners.TryGetValue(target, out events))
                {
                    List<ListenerSubscription> eventListeners = null;
                    if (events.TryGetValue(eventName, out eventListeners))
                    {
                        var toInvoke = eventListeners.ToArray();
                        foreach (var listener in toInvoke)
                        {
                            engine.InvokeMethod(listener.MethodName, null, args);
                        }
                    }
                }
            }

            internal ListenerSubscription Subscribe(string target, string subscriber, string eventName, string listenerName, bool ephemeral)
            {
                if (target == null)
                {
                    target = "";
                }
                ListenerSubscription subscription = new ListenerSubscription(target, subscriber, eventName, listenerName, ephemeral);
                Dictionary<string, List<ListenerSubscription>> events;
                if (!listeners.TryGetValue(subscription.SubscriptionTarget, out events))
                {
                    events = new Dictionary<string, List<ListenerSubscription>>();
                    listeners[subscription.SubscriptionTarget] = events;
                }
                List<ListenerSubscription> eventListeners;
                if (!events.TryGetValue(subscription.Event, out eventListeners))
                {
                    eventListeners = new List<ListenerSubscription>();
                    events[subscription.Event] = eventListeners;
                }
                eventListeners.Add(subscription);
                return subscription;
            }

            internal void Unsubscribe(ListenerSubscription subscription)
            {
                Dictionary<string, List<ListenerSubscription>> events;
                if (listeners.TryGetValue(subscription.SubscriptionTarget, out events))
                {
                    List<ListenerSubscription> eventListeners = null;
                    if (events.TryGetValue(subscription.Event, out eventListeners))
                    {
                        eventListeners.Remove(subscription);
                    }
                }
            }

            internal void Unsubscribe(string subscriber, string eventName, string target)
            {
                if (target == null)
                {
                    target = "";
                }
                Dictionary<string, List<ListenerSubscription>> events;
                if (listeners.TryGetValue(target, out events))
                {
                    List<ListenerSubscription> eventListeners = null;
                    if (events.TryGetValue(eventName, out eventListeners))
                    {
                        eventListeners.Remove(eventListeners.Find(x => x.Event == eventName && x.SubscriberDescription == subscriber && x.SubscriptionTarget == target));
                    }
                }
            }
        }
    }
}