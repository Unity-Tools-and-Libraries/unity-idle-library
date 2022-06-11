using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using io.github.thisisnozaku.idle.framework.Engine;
using static io.github.thisisnozaku.idle.framework.ValueContainer;
using static io.github.thisisnozaku.idle.framework.Engine.IdleEngine;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;

namespace io.github.thisisnozaku.idle.framework
{
    /*
     * A value container holds a value.
     */
    public class ValueContainer : EventSource<ListenerSubscription>
    {
        public delegate object UpdatingMethod(IdleEngine engine, float timeSinceLastUpdate, object previousValue, ValueContainer thisContainer, List<ContainerModifier> modifiersList);

        // The unique id of this container.
        private string internalId;
        private string path;
        private bool Ephemeral;
        public string Path
        {
            get { return path; }
            set
            {
                IdleEngine.ValidatePath(value);
                this.path = value;
                if (this.value is IDictionary<string, ValueContainer>)
                {
                    var children = new List<KeyValuePair<string, ValueContainer>>(ValueAsMap());
                    foreach (var child in children)
                    {
                        child.Value.Path = String.Join(".", this.Path, child.Key);
                    }
                }
            }
        }
        private object cachedFinalValue;

        public ValueContainer SetEphemeral()
        {
            Ephemeral = true;
            return this;
        }

        public ValueContainer SetPermanent()
        {
            Ephemeral = false;
            return this;
        }

        public ValueContainer this[string id]
        {
            get
            {
                if (ValueAsMap() != null)
                {
                    return ValueAsMap()[id];
                }
                return null;
            }
            set
            {
                if (ValueAsMap() == null)
                {
                    throw new InvalidOperationException();
                }
                ValueAsMap()[id] = value;
            }
        }

        public ValueContainer Parent
        {
            get
            {
                var splitPath = Path.Split('.');
                if (splitPath.Length > 2)
                {
                    return Engine.GetProperty(string.Join(".", splitPath.Take(splitPath.Length - 1)));
                }
                else
                {
                    return null;
                }
            }
        }
        // The value held by this container.
        private object value;
        private string type;
        // Method which updates the contained value each tick, if specified.
        private string updatingMethod;
        private string interceptorMethod;
        public string Description;
        private Dictionary<string, ISet<ListenerSubscription>> listeners;
        public IdleEngine Engine { get; private set; }

        private SortedSet<IContainerModifier> modifiers;

        public string Id
        {
            get => internalId;
            internal set
            {
                if (internalId != null)
                {
                    throw new InvalidOperationException();
                }
                internalId = value;
            }
        }

        internal ValueContainer(IdleEngine engine, string id, string startingValue, string description = "", string path = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, BigDouble startingValue, string description = "", string path = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, bool startingValue, string description = "", string path = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, IDictionary<string, ValueContainer> startingValue, string description = "", string path = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, object startingValue, string description, string path, string updater, List<IContainerModifier> startingModifiers, string interceptorMethod)
        {
            this.Id = id;
            this.Path = path;
            this.Engine = engine;
            SetModifiers(startingModifiers);
            if (startingValue is IDictionary<string, ValueContainer>)
            {
                engine.Log(LogType.Log, "Wrapping dictionary in parent notifying dictionary", "engine.internal.container");
                ParentNotifyingDictionary notifyingDictionary = !(startingValue is ParentNotifyingDictionary) ?
                    new ParentNotifyingDictionary(this, startingValue as IDictionary<string, ValueContainer>) :
                    startingValue as ParentNotifyingDictionary;
                notifyingDictionary.SetParent(this);
                startingValue = notifyingDictionary;
            }
            this.value = startingValue;
            this.Description = description != null ? description : "";
            this.updatingMethod = updater;
            this.interceptorMethod = interceptorMethod;
            this.type = DetermineType(this.value);
            this.listeners = new Dictionary<string, ISet<ListenerSubscription>>();
            engine.RegisterMethod(Id + "OnReady", OnReady);
            InternalSubscribe("#" + Id, EngineReadyEvent.EventName, Id + "OnReady", true);
        }

        private object OnReady(IdleEngine engine, ValueContainer container, object ev)
        {
            NotifyImmediately(ValueChangedEvent.EventName, this, null, this.value);
            return null;
        }

        internal static string DetermineType(object value)
        {
            if (value is IDictionary<string, ValueContainer>)
            {
                return "map";
            }
            else if (value is bool)
            {
                return "bool";
            }
            else if (value is string)
            {
                return "string";
            }
            else if (value is BigDouble)
            {
                return "number";
            }
            return "null";
        }

        private object applyModifiers(object v)
        {
            if (v is BigDouble)
            {
                return applyModifiers((BigDouble)v);
            }
            else if (v is bool)
            {
                return applyModifiers((bool)v);
            }
            else if (v is string)
            {
                return applyModifiers(v as string);
            }
            return v;
        }

        private T applyModifiers<T>(T previousValue)
        {
            foreach (var modifier in modifiers)
            {
                if (previousValue != null && modifier.SupportsType(previousValue.GetType()))
                {
                    previousValue = (T)modifier.Apply(Engine, this, previousValue);
                }
            }
            Engine.Log(LogType.Log, String.Format("Caching calculated value in @{0}", Path), "engine.internal.container.cache");
            cachedFinalValue = previousValue;
            return previousValue;
        }



        private void AssertIsRegistered()
        {
            if (Id == null || Path == null)
            {
                Engine.Log(LogType.Error, "ValueContainer is not ready to be used; it must be assigned to a global property in the engine or a descendent of one before use.", "engine.internal.container");
            }
        }

        public bool ValueAsBool()
        {
            AssertIsRegistered();
            if (cachedFinalValue != null)
            {
                Engine.Log(LogType.Log, string.Format("Using cached value of @{0} as bool", Path), "engine.internal.container");
                return CoerceToBool(cachedFinalValue);
            }
            return applyModifiers(CoerceToBool(value));
        }

        public BigDouble ValueAsNumber()
        {
            AssertIsRegistered();
            if (cachedFinalValue != null)
            {
                Engine.Log(LogType.Log, string.Format("Using cached value of @{0} as number", Path), "engine.internal.container");
                return CoerceToNumber(cachedFinalValue);
            }
            return applyModifiers(CoerceToNumber(value));
        }

        public string ValueAsString()
        {
            AssertIsRegistered();
            if (cachedFinalValue != null)
            {
                Engine.Log(LogType.Log, string.Format("Using cached value of @{0} as string", Path), "engine.internal.container");
                return CoerceToString(cachedFinalValue);
            }
            return applyModifiers(CoerceToString(value));
        }

        public IDictionary<string, ValueContainer> ValueAsMap()
        {
            AssertIsRegistered();
            if (cachedFinalValue != null)
            {
                Engine.Log(LogType.Log, string.Format("Using cached value of @{0} as map", Path), "engine.internal.container");
                return CoerceToMap(cachedFinalValue);
            }
            return applyModifiers(CoerceToMap(value));
        }

        public object ValueAsRaw()
        {
            return value;
        }

        public ValueContainer GetProperty(string path)
        {
            return Engine.GetProperty(string.Join(".", Path, path));
        }

        public override bool Equals(object other)
        {
            if (other is ValueContainer)
            {
                ValueContainer otherValueReference = other as ValueContainer;
                return otherValueReference.value.GetType() == this.value.GetType() &&
                    otherValueReference.value.Equals(this.value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            AssertIsRegistered();
            return internalId.GetHashCode() ^ (value != null ? value.GetHashCode() : 0);
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            AssertIsRegistered();
            if (this.updatingMethod != null)
            {
                // TODO: Normalize
                var updateOut = engine.InvokeMethod(this.updatingMethod, this, deltaTime, this.value);
                bool outputIsValid = false;
                if (updateOut is int)
                {
                    outputIsValid = true;
                    updateOut = new BigDouble((int)updateOut);
                }
                else if (updateOut is long)
                {
                    outputIsValid = true;
                    updateOut = new BigDouble((long)updateOut);
                }
                else if (updateOut is float)
                {
                    outputIsValid = true;
                    updateOut = new BigDouble((float)updateOut);
                }
                else if (updateOut is double)
                {
                    outputIsValid = true;
                    updateOut = new BigDouble((double)updateOut);
                }
                if (updateOut is bool || updateOut is string || updateOut is ParentNotifyingDictionary || updateOut is BigDouble || updateOut is null)
                {
                    outputIsValid = true;
                }
                if (outputIsValid)
                {
                    setInternal(updateOut);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Type returned from updating function was {0}, which is not valid.", updateOut.GetType().ToString()));
                }
            }
            if (this.value is IDictionary<string, ValueContainer>)
            {
                foreach (var child in ((IDictionary<string, ValueContainer>)this.value))
                {
                    child.Value.Update(engine, deltaTime);
                }
            }
            bool allModifiersCached = true;
            foreach (var modifier in modifiers)
            {
                allModifiersCached = allModifiersCached && modifier.IsCached;
            }
            if (!allModifiersCached)
            {
                cachedFinalValue = null;
            }
            NotifyImmediately(ValueContainerUpdatedEvent.EventName, this);
        }

        private object setInternal(object newValue)
        {
            var old = this.value;
            this.value = this.interceptorMethod != null ? Engine.InvokeMethod(this.interceptorMethod, this, this, this.value, newValue) : newValue;
            NotifyChange(old, this.value);
            if (cachedFinalValue != null && !object.Equals(old, this.value))
            {
                Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to base value being set", Path), "engine.internal.container.cache");
                cachedFinalValue = null;
            }
            return this.value;
        }

        private void NotifyChange(object oldValue, object newValue)
        {
            NotifyImmediately(ValueChangedEvent.EventName, this, oldValue, newValue);
            var splitPath = Path.Split('.');
            if (Parent != null)
            {
                Parent.NotifyImmediately(ChildValueChangedEvent.EventName, this, oldValue, newValue);
            }
        }

        public BigDouble Set(BigDouble newValue)
        {
            AssertCanSet();
            return CoerceToNumber(setInternal(newValue));
        }

        public string Set(string newValue)
        {
            AssertCanSet();
            return CoerceToString(setInternal(newValue));
        }

        public bool Set(bool newValue)
        {
            AssertCanSet();
            return CoerceToBool(setInternal(newValue));
        }

        public ValueContainer Set(IDictionary<string, ValueContainer> newValue)
        {
            if (newValue != null)
            {
                Engine.Log(LogType.Log, "Wrapping dictionary in parent notifying dictionary", "engine.internal.container");
                newValue = new ParentNotifyingDictionary(this, newValue as IDictionary<string, ValueContainer>);
            }
            AssertCanSet();
            setInternal(newValue);
            return this;
        }

        public IReadOnlyCollection<IContainerModifier> GetModifiers()
        {
            return modifiers.ToList().AsReadOnly();
        }

        private void DependencyCacheInvalidated()
        {
            Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to cache change in modifiers", Path), "engine.internal.container.cache");
            cachedFinalValue = null;
        }

        internal void SetModifiers(List<IContainerModifier> modifiers)
        {
            if (this.modifiers != null)
            {
                foreach (var modifier in this.modifiers)
                {
                    var valueMod = modifier as ValueModifier;
                    if (valueMod != null)
                    {
                        valueMod.CacheChanged -= DependencyCacheInvalidated;
                    }
                }
            }
            this.modifiers = modifiers != null ? new SortedSet<IContainerModifier>(modifiers, new ContainerModifierComparer()) : new SortedSet<IContainerModifier>(new ContainerModifierComparer());
            foreach (var modifier in this.modifiers)
            {
                modifier.OnAdd(Engine, this);
                var valueMod = modifier as ValueModifier;
                if (valueMod != null)
                {
                    valueMod.CacheChanged += DependencyCacheInvalidated;
                }
            }
            if (cachedFinalValue != null)
            {
                Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to change in modifiers", Path), "engine.internal.container.cache");
            }
            cachedFinalValue = null;
        }

        internal void DoNotification(string eventName, params object[] args)
        {
            ISet<ListenerSubscription> listeners;
            if (this.listeners.TryGetValue(eventName, out listeners))
            {
                listeners = new HashSet<ListenerSubscription>(listeners); // FIXM: Ew gross yuck.
                foreach (var listener in listeners)
                {
                    DoListenerInvocation(listener, eventName, args);
                }
            }
        }

        internal void Broadcast(string eventName, params object[] args)
        {
            Engine.Log(LogType.Log, "Broadcasting " + eventName + " from #" + Id + ".", "engine.internal.container");
            DoNotification(eventName, args);
            if (ValueAsMap() != null)
            {
                foreach (var child in ValueAsMap())
                {
                    child.Value.Broadcast(eventName, args);
                }
            }
        }

        public void NotifyImmediately(string eventName, params object[] args)
        {
            if (Engine.IsReady)
            {
                Engine.Log(LogType.Log, string.Format("[event.{0}] Notifying {1}@{2}.", eventName, Id, Path), "engine.internal.container");
                AssertIsRegistered();
                DoNotification(eventName, args);
                foreach (var mod in modifiers)
                {
                    mod.Trigger(Engine, eventName);
                }
                Engine.BubbleEvent(eventName, this, args);
            }
            else
            {
                Engine.Log(LogType.Warning, "Start() has not been called on the engine.", "engine.internal.container");
            }
        }

        public ValueContainer SetUpdater(string p)
        {
            updatingMethod = p;
            return this;
        }

        public ValueContainer SetUpdater(UserMethod listener)
        {
            return SetUpdater(listener.Method.Name);
        }

        public ValueContainer SetInterceptor(string name)
        {
            interceptorMethod = name;
            return this;
        }

        public ValueContainer SetInterceptor(UserMethod interceptor)
        {
            return SetInterceptor(interceptor.Method.Name);
        }

        public static implicit operator BigDouble(ValueContainer container)
        {
            return container.ValueAsNumber();
        }

        public static implicit operator string(ValueContainer valueReference)
        {
            return valueReference.ValueAsString();
        }

        public static implicit operator bool(ValueContainer valueReference)
        {
            return valueReference.ValueAsBool();
        }

        public override string ToString()
        {
            string valueType = "unknown";
            string valueString = value != null ? value.ToString() : "null";
            if (this.value is BigDouble)
            {
                valueType = "number";
            }
            else if (this.value is bool)
            {
                valueType = "boolean";
            }
            else if (this.value is string)
            {
                valueType = "string";
                valueString = string.Format("'{0}'", valueString);
            }
            else if (this.value is IDictionary<string, ValueContainer>)
            {
                valueString = valueType = "map";
            }
            return string.Format("Reference #{4} @{0}:{1} (containing {2}{3})", Path, this.Description != "" ? " " + this.Description : "", valueType, valueType != "map" ? " " + valueString : "", Id);
        }

        public ValueContainer AddModifier(IContainerModifier modifier)
        {
            var old = this.ValueAsRaw();
            if (this.modifiers.Add(modifier))
            {
                var valueMod = modifier as ValueModifier;
                if (valueMod != null)
                {
                    valueMod.CacheChanged += DependencyCacheInvalidated;
                }
                modifier.OnAdd(Engine, this);
                NotifyChange(old, this.ValueAsRaw());
                Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to change in modifiers", Path), "engine.internal.container.cache");
                cachedFinalValue = null;
            }
            
            return this;
        }

        public ValueContainer RemoveModifier(IContainerModifier modifier)
        {
            var old = this.ValueAsRaw();
            bool removed = this.modifiers.Remove(modifier);
            if (removed)
            {
                modifier.OnRemove(Engine, this);
            }
            NotifyChange(old, this.ValueAsRaw());
            Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to change in modifiers", Path), "engine.internal.container.cache");
            cachedFinalValue = null;
            return this;
        }

        public ListenerSubscription Subscribe(string subscriberDescription, string eventName, string eventHandlerName, bool ephemeral = false)
        {
            var listener = InternalSubscribe(subscriberDescription, eventName, eventHandlerName, ephemeral);
            if (eventName == ValueChangedEvent.EventName || eventName == EngineReadyEvent.EventName)
            {
                if (Engine.IsReady)
                {
                    DoListenerInvocation(listener, eventName, this, this.value, this.value);
                }
            }
            return listener;
        }

        public ListenerSubscription Subscribe(string subscriberDescription, string eventName, UserMethod eventHandler, bool ephemeral = false)
        {
            return Subscribe(subscriberDescription, eventName, eventHandler.Method.Name, ephemeral);
        }

        private ListenerSubscription InternalSubscribe(string subscriberDescription, string eventName, string eventhandlerMethodName, bool ephemeral = false)
        {
            ISet<ListenerSubscription> listeners;
            if (!this.listeners.TryGetValue(eventName, out listeners))
            {
                this.listeners[eventName] = listeners = new HashSet<ListenerSubscription>();
            }
            var subscription = new ListenerSubscription(subscriberDescription, eventName, eventhandlerMethodName, ephemeral);
            listeners.Add(subscription);
            return subscription;
        }

        private void DoListenerInvocation(ListenerSubscription listener, string eventName, params object[] args)
        {
            try
            {
                Engine.Log(LogType.Log, "Invoking listener " + listener + " from #" + Id + ".", "engine.internal.container");
                Engine.InvokeMethod(listener.MethodName, this, args);
            }
            catch (Exception e)
            {
                Engine.Log(LogType.Error, string.Format("Error trying to invoke listener from \"{2}\" for event \"{0}\": {1}.", eventName, e.ToString(), listener.SubscriberDescription), "engine.internal.container");
            }
        }

        public void Unsubscribe(ListenerSubscription subscription)
        {
            ISet<ListenerSubscription> listeners;
            if (this.listeners.TryGetValue(subscription.Event, out listeners))
            {
                listeners.Remove(subscription);
            }
        }

        public void Unsubscribe(string subscriber, string eventName)
        {
            ISet<ListenerSubscription> listeners;
            if (this.listeners.TryGetValue(eventName, out listeners))
            {
                ListenerSubscription subscription = listeners.Where(l => l.SubscriberDescription == subscriber && l.Event == eventName)
                    .FirstOrDefault(null);
                if (subscription != null)
                {
                    listeners.Remove(subscription);
                }
            }
        }

        private void AssertCanSet()
        {

        }

        public static bool CoerceToBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            else if (value is IDictionary<string, ValueContainer>)
            {
                return true;
            }
            else if (value == null)
            {
                return false;
            }
            else if (value is string)
            {
                return (string)value != "";
            }
            else
            {
                return (BigDouble)value != BigDouble.Zero;
            }
        }

        public static BigDouble CoerceToNumber(object value)
        {
            if (value is bool)
            {
                return (bool)value ? BigDouble.One : BigDouble.Zero;
            }
            else if (value is IDictionary<string, ValueContainer> || value == null)
            {
                return BigDouble.Zero;
            }
            else if(value is string)
            {
                try
                {
                    return BigDouble.Parse(value as string);
                } catch(Exception ex)
                {
                    return BigDouble.NaN;
                }
            }
            else
            {
                return (BigDouble)value;
            }
        }
        public static string CoerceToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            else if (value == null)
            {
                return "";
            }
            return value.ToString();
        }
        public static IDictionary<string, ValueContainer> CoerceToMap(object value)
        {
            if (value is BigDouble || value is string || value is bool || value is Func<IdleEngine, ValueContainer, object[], object> || value == null)
            {
                return null;
            }
            if (!(value is IDictionary<string, ValueContainer>))
            {
                throw new InvalidOperationException(string.Format("Failed to coerce a value of type {0} to IDictionary<string, ValueReference>.", value.GetType()));
            }
            return (IDictionary<string, ValueContainer>)value;
        }

        public static object NormalizeValue(object value)
        {
            if (value is BigDouble || value is string || value is bool || value is IDictionary<string, ValueContainer> || value is ValueContainer)
            {
                return value;
            }
            if (value is int)
            {
                return new BigDouble((int)value);
            }
            if (value is long)
            {
                return new BigDouble((long)value);
            }
            if (value is float)
            {
                return new BigDouble((float)value);
            }
            if (value is double)
            {
                return new BigDouble((double)value);
            }
            if (value is decimal)
            {
                return BigDouble.Parse(value.ToString());
            }
            if (value is null)
            {
                return null;
            }
            throw new InvalidOperationException();
        }

        public void RestoreFromSnapshot(IdleEngine engine, Snapshot snapshot)
        {
            engine.RegisterMethod(Id + "OnReady", OnReady);
            if (snapshot.Path != this.Path)
            {
                throw new InvalidOperationException(String.Format("Tried to restore container with path '{0}' from a snapshot with path '{1}'", this.Path, snapshot.Path));
            }
            switch (snapshot.Type)
            {
                case "string":
                    Set((string)snapshot.Value);
                    break;
                case "bool":
                    Set((bool)snapshot.Value);
                    break;
                case "number":
                    Set(JsonConvert.DeserializeObject<BigDouble>((snapshot.Value as JToken).ToString()));
                    break;
                case "map":
                    var deserialized = JsonConvert.DeserializeObject<IDictionary<string, ValueContainer.Snapshot>>(((JObject)snapshot.Value).ToString());
                    var map = new Dictionary<string, ValueContainer>();
                    foreach (var child in deserialized)
                    {
                        if (child.Value != null)
                        {
                            var container = Engine.GetOrCreateContainerByPath(child.Value.Path, false);
                            container.RestoreFromSnapshot(engine, child.Value);
                            map[child.Key] = container;
                        }
                    }
                    Set(map);
                    break;
                case "null":
                    Set((string)null);
                    break;
            }
            if (snapshot.Listeners != null)
            {
                listeners = snapshot.Listeners.ToDictionary(e => e.Key, e => (ISet<ListenerSubscription>)new HashSet<ListenerSubscription>(e.Value));
            }
            InternalSubscribe("#" + Id, EngineReadyEvent.EventName, Id + "OnReady", true);
        }

        // TODO: Move into own file
        public class Snapshot
        {
            public string Type;
            public string Path;
            public object Value;
            public string UpdateMethod;
            public Dictionary<string, List<ListenerSubscription>> Listeners;

            public Snapshot()
            {

            }

            public Snapshot(string path, object value, string updateMethod, Dictionary<string, ISet<ListenerSubscription>> listeners)
            {
                if (path == null)
                {
                    throw new ArgumentNullException("path");
                }
                Path = path;
                Type = DetermineType(value);
                Value = Type == "map" ? (value as IDictionary<string, ValueContainer>)
                    .ToDictionary(x =>
                    {
                        return x.Key;
                    }, x =>
                    {
                        return x.Value.GetSnapshot();
                    }) :
                    (value != null ? value : "null");

                Listeners = listeners.ToDictionary(e => e.Key, e => e.Value.Where(l => !l.Ephemeral).ToList());
                UpdateMethod = updateMethod;
            }
        }

        public Snapshot GetSnapshot()
        {
            if (!Ephemeral)
            {
                return new Snapshot(Path, value, updatingMethod, listeners);
            }
            return null;
        }

        public class ListenerSubscription // TODO: Move to own file
        {
            public readonly string MethodName;
            public readonly string Event;
            public readonly bool Ephemeral;
            public readonly string SubscriberDescription;

            public ListenerSubscription(string subscriberDescription, string eventName, string methodName, bool ephemeral)
            {
                Event = eventName;
                SubscriberDescription = subscriberDescription;
                MethodName = methodName;
                Ephemeral = ephemeral;
            }

            public override bool Equals(object obj)
            {
                return obj is ListenerSubscription subscription &&
                       MethodName == subscription.MethodName &&
                       Ephemeral == subscription.Ephemeral &&
                       SubscriberDescription == subscription.SubscriberDescription;
            }

            public override int GetHashCode()
            {
                int hashCode = -119410202;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MethodName);
                hashCode = hashCode * -1521134295 + Ephemeral.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SubscriberDescription);
                return hashCode;
            }
        }

        public static class Context
        {
            public delegate IDictionary<string, object> ContextGenerator(IdleEngine engine, ValueContainer container);

            public static readonly ContextGenerator DefaultGenerator = (ie, vc) =>
            {
                return new Dictionary<string, object>()
                {
                    { "this", vc }
                };
            };
            public static readonly ContextGenerator ParentGenerator = (ie, vc) =>
            {
                return new Dictionary<string, object>()
                {
                    { "this", vc.Parent }
                };
            };
            public static readonly ContextGenerator GlobalContextGenerator = (ie, vc) =>
            {
                return ie.GenerateGlobalContext();
            };
        }
    }
}