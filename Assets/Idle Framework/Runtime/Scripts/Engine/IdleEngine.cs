using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
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
    public partial class IdleEngine : RandomNumberSource, EventSource<ListenerSubscription>, IDefinitionManager

    {
        private System.Random random;
        public delegate object UserMethod(IdleEngine engine, ValueContainer container, params object[] args);

        private static Regex PathRegex = new Regex("[a-zA-Z0-9]+(\\.[a-zA-Z0-9]+)*");

        private IDictionary<string, ValueContainer> references = new Dictionary<string, ValueContainer>();
        public bool IsReady { get; private set; }
        private readonly IDictionary<string, ValueContainer> globalProperties = new Dictionary<string, ValueContainer>();
        private Dictionary<string, List<ListenerSubscription>> listeners = new Dictionary<string, List<ListenerSubscription>>();
        private Dictionary<string, UserMethod> methods = new Dictionary<string, UserMethod>();
        private DefinitionManager definitions = new DefinitionManager();


        public static void ValidatePath(string path)
        {
            if (path != null && !PathRegex.Match(path).Success)
            {
                throw new ArgumentException(path);
            }
        }

        public IdleEngine(GameObject gameObject)
        {
            random = new System.Random();
            UserData.RegisterProxyType<ValueContainerScriptProxy, ValueContainer>(c => new ValueContainerScriptProxy(c));
            UserData.RegisterType<BigDouble>();
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Number, typeof(BigDouble), (arg) =>
            {
                return BigDouble.Parse(arg.CastToString());
            });
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.UserData, typeof(BigDouble), (arg) =>
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
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(typeof(BigDouble), (script, arg) =>
            {
                return UserData.Create(arg);
            });
        }

        public void Start()
        {
            Debug.Log("Starting engine");
            IsReady = true;
            Broadcast(EngineReadyEvent.EventName);
        }

        // Event Management
        public void Broadcast(string eventName, params object[] args)
        {
            Debug.Log("Broadcasting " + eventName);
            NotifyImmediately(eventName, args);
            foreach (var prop in globalProperties)
            {
                if (prop.Value != null)
                {
                    prop.Value.Broadcast(eventName, args);
                }
            }
        }

        public void NotifyImmediately(string eventName, params object[] arguments)
        {
            List<ListenerSubscription> listeners = null;
            if (this.listeners.TryGetValue(eventName, out listeners))
            {
                {
                    foreach (var listener in listeners)
                    {
                        InvokeMethod(listener.MethodName, null, arguments);
                    }
                }
            }
        }

        public void Unsubscribe(ListenerSubscription subscription)
        {
            List<ListenerSubscription> listeners;
            if (this.listeners.TryGetValue(subscription.Event, out listeners))
            {
                listeners.Remove(subscription);
            }
        }

        public void Unsubscribe(string subscriber, string eventName)
        {
            List<ListenerSubscription> listeners;
            if (this.listeners.TryGetValue(eventName, out listeners))
            {
                ListenerSubscription subscription = listeners.Where(l => l.SubscriberDescription == subscriber && l.Event == eventName)
                    .FirstOrDefault();
                if (subscription != null)
                {
                    listeners.Remove(subscription);
                }
            }
        }

        public ListenerSubscription Subscribe(string subscriber, string eventName, string listener, bool ephemeral = false)
        {
            List<ListenerSubscription> eventListeners;
            if (!this.listeners.TryGetValue(eventName, out eventListeners))
            {
                eventListeners = new List<ListenerSubscription>();
                listeners[eventName] = eventListeners;
            }
            var subscription = new ListenerSubscription(subscriber, eventName, listener, ephemeral);
            eventListeners.Add(subscription);
            return subscription;
        }

        public ListenerSubscription Subscribe(string subscriber, string eventName, UserMethod listener, bool ephemeral = false)
        {
            return Subscribe(subscriber, eventName, listener.Method.Name, ephemeral);
        }

        internal void BubbleEvent(string eventName, ValueContainer valueContainer, params object[] args)
        {
            var sourcePath = valueContainer.Path;
            var sourceTokens = sourcePath.Split('.');
            for (int i = sourceTokens.Length - 1; i > 0; i--)
            {
                string targetPath = sourceTokens[0];
                for (int t = 1; t < i; t++)
                {
                    targetPath += "." + sourceTokens[t];
                }
                var targetContainer = GetProperty(targetPath);
                if (targetContainer != null && targetContainer != valueContainer)
                {
                    targetContainer.DoNotification(eventName, args);
                }
            }
        }


        // Random Number
        public int RandomInt(int count)
        {
            return random.Next(count);
        }

        // Global Properties
        public ValueContainer GetProperty(string path)
        {
            return DoGetProperty(path);
        }

        public enum GetOperationType
        {
            IGNORE_MISSING, // If a path reaches a non-existant container or an intermediate container which is not a dictionary, return null
            CREATE_MISSING, // If a path reaches a non-existant container, create it; if it reaches a non-dictionary intermediate container, return null.
            ASSERT_CORRECT  // If a path reaches a non-existant container or an intermediate container which is not a dictionary, throw an exception
        }

        private ValueContainer DoGetProperty(string path, GetOperationType operationType = GetOperationType.IGNORE_MISSING)
        {
            var tokens = path.Split('.');
            ValueContainer container = null;
            string subpath = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                if (i == 0)
                {
                    subpath = token;
                    if (!globalProperties.TryGetValue(subpath, out container))
                    {
                        if (operationType == GetOperationType.CREATE_MISSING)
                        {
                            container = globalProperties[token] = CreateValueContainer(path: subpath);
                            globalProperties.TryGetValue(token, out container);
                        }
                        else if (operationType == GetOperationType.ASSERT_CORRECT)
                        {
                            throw new InvalidOperationException("While performing a strict traversal operation, a null container was found within the path at " + subpath);
                        }
                    }
                }
                else
                {
                    subpath = String.Join(".", subpath, token);
                    if (token == "^")
                    {
                        if (container.Parent == null)
                        {
                            throw new InvalidOperationException("Tried to go up the path hierarchy but were already at the top.");
                        }
                        container = container.Parent;
                    }
                    else if (container == null)
                    {
                        if (operationType == GetOperationType.ASSERT_CORRECT)
                        {
                            throw new ArgumentException(String.Format("Found null container at {0}", subpath));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    var map = container.ValueAsMap();
                    if (map == null)
                    {
                        if (container.ValueAsRaw() != null && operationType == GetOperationType.ASSERT_CORRECT)
                        {
                            throw new InvalidOperationException(String.Format("Found a container at path \"{0}\" which is a non-null, non-dictionary value but is trying to be set as a parent of another value.", subpath));
                        }
                        else if (operationType == GetOperationType.CREATE_MISSING)
                        {
                            map = container.Set(new Dictionary<string, ValueContainer>() { }).ValueAsMap();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    if (token != "^" && !map.TryGetValue(token, out container) && operationType == GetOperationType.CREATE_MISSING)
                    {
                        container = map[token] = CreateValueContainer(path: subpath);
                    }
                }
            }
            return container;
        }

        public ValueContainer SetProperty(string property, bool value, string description = "", List<ContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var container = GetOrCreateContainerByPath(property);
            container.SetInterceptor(interceptor);
            container.Path = property;
            container.Set(value);
            container.SetUpdater(updater);
            container.SetModifiers(modifiers);
            return container;
        }

        public ValueContainer SetProperty(string property, string value = null, string description = "", List<ContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var container = GetOrCreateContainerByPath(property);
            container.SetInterceptor(interceptor);
            container.Path = property;
            container.Set(value);
            container.SetModifiers(modifiers);
            container.SetUpdater(updater);
            return container;
        }

        public ValueContainer SetProperty(string property, IDictionary<string, ValueContainer> value, string description = "", List<ContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var container = GetOrCreateContainerByPath(property);
            container.SetInterceptor(interceptor);
            container.Path = property;
            container.Set(value);
            container.SetModifiers(modifiers);
            container.SetUpdater(updater);
            return container;
        }

        public ValueContainer SetProperty(string property, BigDouble value, string description = "", List<ContainerModifier> modifiers = null, string updater = null, string interceptor = null)
        {
            var container = GetOrCreateContainerByPath(property);
            container.SetInterceptor(interceptor);
            container.Path = property;
            container.Set(value);
            container.SetModifiers(modifiers);
            container.SetUpdater(updater);
            return container;
        }

        public ValueContainer GetOrCreateContainerByPath(string path, bool assertParentsAreDictionary = true)
        {
            return DoGetProperty(path, operationType: GetOperationType.CREATE_MISSING);
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

        public ValueContainer CreateValueContainer(string value = null, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updater = null)
        {
            var vc = new ValueContainer(this, NextId(), value, description, path, modifiers, updater);
            references[vc.Id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(BigDouble value, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updater = null)
        {
            var vc = new ValueContainer(this, NextId(), value, description, path, modifiers, updater);
            references[vc.Id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(bool value, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updater = null)
        {
            var vc = new ValueContainer(this, NextId(), value, description, path, modifiers, updater);
            references[vc.Id] = vc;
            return vc;
        }

        public ValueContainer CreateValueContainer(IDictionary<string, ValueContainer> value, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updater = null)
        {
            var vc = new ValueContainer(this, NextId(), value, description, path, modifiers, updater);
            references[vc.Id] = vc;
            return vc;
        }
        private static Regex PlaceholderRegex = new Regex("\\$\\{(.+?)\\}");

        private Dictionary<string, Script> CachedScripts;

        private Func<ScriptExecutionContext, CallbackArguments, DynValue> WrapMethod(string methodName)
        {
            return (ctx, args) =>
            {
                var methodOut = NormalizeValue(methods[methodName].Invoke(this, null, args.GetArray().Select(arg => ValueContainer.NormalizeValue(arg.ToObject())).ToArray()));
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

        private IDictionary<string, object> GetGlobalContext()
        {
            var properties = new Dictionary<string, object>();
            foreach (var method in methods)
            {
                properties[method.Key] = DynValue.NewCallback(WrapMethod(method.Key));
            }
            foreach (var global in globalProperties)
            {
                properties[global.Key] = global.Value;
            }
            return properties;
        }

        public object EvaluateExpression(string valueExpression, IDictionary<string, object> context = null)
        {
            context = context != null ? context : GetGlobalContext();
            var script = new Script();
            foreach (var property in context)
            {
                script.Globals[property.Key] = property.Value;
            }
            return NormalizeValue(script.DoString("return " + valueExpression).ToObject());
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
            return new Snapshot(globalProperties.ToDictionary(x => x.Value.Path, x => x.Value.GetSnapshot()));
        }

        public void RestoreFromSnapshot(Snapshot snapshot)
        {
            foreach (var e in snapshot.GlobalProperties)
            {
                if (e.Value != null)
                {
                    GetOrCreateContainerByPath(e.Value.Path).RestoreFromSnapshot(this, e.Value);
                }
            }
        }

        public class Snapshot
        {
            public readonly Dictionary<string, ValueContainer.Snapshot> GlobalProperties;

            public Snapshot(Dictionary<string, ValueContainer.Snapshot> globalProperties)
            {
                GlobalProperties = globalProperties;
            }
        }

        public object InvokeMethod(string methodName, ValueContainer container, params object[] arg)
        {
            UserMethod method;
            if (methods.TryGetValue(methodName, out method))
            {
                return method(this, container, arg);
            }
            else
            {
                throw new InvalidOperationException("Couldn't find method " + methodName + ". Ensure that the method is registered before use.");
            }
        }

        public object InvokeMethod(UserMethod method, ValueContainer container, params object[] args)
        {
            return InvokeMethod(method.Method.Name, container, args);
        }

        public void RegisterMethod(UserMethod method)
        {
            RegisterMethod(method.Method.Name, method);
        }

        public void RegisterMethod(string name, UserMethod method)
        {
            methods[name] = method;
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

        /*
         * Transform a definition into a dictionary containing the definition properties.
         */
        public ValueContainer InstantiateDefinition(IDefinition definition)
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
            newModule.SetEngineProperties(this);
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
    }
}