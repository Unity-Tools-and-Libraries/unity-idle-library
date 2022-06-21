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
        private bool Ephemeral;
        private string path;
        public string Path
        {
            get { return path; }
            internal set
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
                else if (this.value is IList<ValueContainer>)
                {
                    var children = new List<ValueContainer>(ValueAsList());
                    for (int i = 0; i < children.Count; i++)
                    {
                        children[i].Path = String.Join(".", this.Path, i);
                    }
                }
                cachedParentValue = null;
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
        private ValueContainer cachedParentValue = null;
        public ValueContainer Parent { get; set; }
        // The value held by this container.
        private object value;
        private string type;
        // Method which updates the contained value each tick, if specified.
        private string updatingMethod;
        private string interceptorMethod;
        public string Description;
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

        internal ValueContainer(IdleEngine engine, string id, string startingValue, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, parent, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, BigDouble startingValue, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, parent, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, bool startingValue, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, parent, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, IDictionary<string, ValueContainer> startingValue, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, parent, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, IList<ValueContainer> startingValue, string description = "", ValueContainer parent = null, List<IContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, parent, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, object startingValue, string description, ValueContainer parent, string updater, List<IContainerModifier> startingModifiers, string interceptorMethod)
        {
            this.Id = id;
            this.Parent = parent;
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
            else if (startingValue is IList<ValueContainer>)
            {
                engine.Log(LogType.Log, "Wrapping dictionary in parent notifying dictionary", "engine.internal.container");
                ParentNotifyingList notifyingList = !(startingValue is ParentNotifyingList) ?
                    new ParentNotifyingList(this, startingValue as IList<ValueContainer>) :
                    startingValue as ParentNotifyingList;
                notifyingList.SetParent(this);
                startingValue = notifyingList;
            }
            this.interceptorMethod = interceptorMethod;
            this.Description = description != null ? description : "";
            this.updatingMethod = updater;
            engine.RegisterMethod(Id + "OnReady", OnReady);
            InternalSubscribe("#" + Id, EngineReadyEvent.EventName, Id + "OnReady", true);

            setInternal(startingValue);
        }

        private object OnReady(IdleEngine engine, object ev)
        {
            NotifyImmediately("on_ready", this, null, this.value); // FIXME: literal event name.
            return null;
        }

        internal static string DetermineType(object value)
        {
            if (value is IDictionary<string, ValueContainer> || value is IDictionary<string, ValueContainer.Snapshot>)
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
            else if (value is IList<ValueContainer> || value is IList<Snapshot>)
            {
                return "list";
            }
            return "null";
        }

        internal string GetInterceptor()
        {
            return interceptorMethod;
        }

        internal string GetUpdater()
        {
            return updatingMethod;
        }

        private T applyModifiers<T>(T previousValue)
        {
            foreach (var modifier in modifiers)
            {
                if (previousValue != null && modifier.CanApply(previousValue))
                {
                    previousValue = (T)modifier.Apply(Engine, this, previousValue);
                }
            }
            Engine.Log(LogType.Log, String.Format("Caching calculated value in @{0}", Path), "engine.internal.container.cache");
            SetCachedValue(previousValue);
            return previousValue;
        }



        private void AssertIsRegistered()
        {
            if (Id == null || Path == null)
            {
                //Engine.Log(LogType.Error, "ValueContainer is not ready to be used; it must be assigned to a global property in the engine or a descendent of one before use.", "engine.internal.container");
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

        public IList<ValueContainer> ValueAsList()
        {
            AssertIsRegistered();
            if (cachedFinalValue != null)
            {
                Engine.Log(LogType.Log, string.Format("Using cached value of @{0} as list", Path), "engine.internal.container");
                return CoerceToList(cachedFinalValue);
            }
            return applyModifiers(CoerceToList(value));
        }

        public object ValueAsRaw()
        {
            return value;
        }

        public ValueContainer GetProperty(string path, GetOperationType operationType = GetOperationType.GET_OR_NULL)
        {
            var tokens = path.Split('.');
            string subpath = String.Join(".", tokens.Skip(1));
            ValueContainer child = null;
            if (operationType == GetOperationType.GET_OR_CREATE && DataType == "null")
            {
                setInternal(new Dictionary<string, ValueContainer>());
            }

            if (tokens[0] == "^")
            {
                if (Parent == null)
                {
                    throw new InvalidOperationException();
                }
                if (tokens.Length > 1)
                {
                    return Parent.GetProperty(subpath);
                }
                return Parent;
            }
            else if (DetermineType(value) == "map")
            {
                var thisMap = this.ValueAsMap();
                if (!thisMap.TryGetValue(tokens[0], out child))
                {
                    switch (operationType)
                    {
                        case GetOperationType.GET_OR_THROW:
                            throw new InvalidOperationException();
                        case GetOperationType.GET_OR_NULL:
                            return null;
                        case GetOperationType.GET_OR_CREATE:
                            child = thisMap[tokens[0]] = Engine.CreateValueContainer(tokens.Length > 1 ? new Dictionary<string, ValueContainer>() : null, parent: this);
                            if (Path != null)
                            {
                                child.Path = string.Join(".", Path, tokens[0]);
                            }
                            break;
                    }
                }
            }
            else if (DetermineType(value) == "list")
            {
                var thisList = ValueAsList();
                int parsedIndex = int.Parse(tokens[0]);
                child = thisList[parsedIndex];
            }

            if (child == null)
            {
                switch (operationType)
                {
                    case GetOperationType.GET_OR_NULL:
                        return null;
                    case GetOperationType.GET_OR_THROW:
                        throw new InvalidOperationException();
                    case GetOperationType.GET_OR_CREATE:
                        child = Engine.CreateValueContainer(parent: this);
                        child.Path = String.Join(".", Path, tokens[0]);
                        break;
                }

            }
            if (tokens.Length == 1)
            {
                return child;
            }
            else
            {
                return child.GetProperty(subpath, operationType);
            }
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            AssertIsRegistered();
            if (this.updatingMethod != null)
            {
                // TODO: Normalize
                var updateOut = engine.InvokeMethod(this.updatingMethod, this, new BigDouble(deltaTime), this.value);
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
                if (updateOut is bool || updateOut is string || updateOut is IDictionary<string, ValueContainer> || updateOut is BigDouble || updateOut is null || updateOut is IList<ValueContainer>)
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
                ClearCachedValue();
            }
            NotifyImmediately(ValueContainerUpdatedEvent.EventName, this);
        }

        private object setInternal(object newValue)
        {
            var old = this.value;
            if (newValue is IDictionary<string, ValueContainer>)
            {
                newValue = new ParentNotifyingDictionary(this, newValue as IDictionary<string, ValueContainer>);
            }
            else if (newValue is IList<ValueContainer>)
            {
                newValue = new ParentNotifyingList(this, newValue as IList<ValueContainer>);
            }
            this.value = NormalizeValue(this.interceptorMethod != null ? Engine.InvokeMethod(this.interceptorMethod, this, this, this.value, newValue) : newValue);

            NotifyChange(old, this.value);
            if (cachedFinalValue != null && !object.Equals(old, this.value))
            {
                Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to base value being set", Path), "engine.internal.container.cache");
                ClearCachedValue();
            }
            type = DetermineType(value);
            return this.value;
        }

        private void NotifyChange(object oldValue, object newValue)
        {
            NotifyImmediately(ValueChangedEvent.EventName, this, oldValue, newValue);
            if (Parent != null)
            {
                Parent.NotifyImmediately(ChildValueChangedEvent.EventName, this, oldValue, newValue);
            }
        }
        public object Set(object newValue)
        {
            AssertCanSet();
            return setInternal(NormalizeValue(newValue));
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

        public IDictionary<string, ValueContainer> Set(IDictionary<string, ValueContainer> newValue)
        {
            AssertCanSet();
            return CoerceToMap(setInternal(newValue));
        }

        public IList<ValueContainer> Set(IList<ValueContainer> newValue)
        {
            AssertCanSet();
            return CoerceToList(setInternal(newValue));
        }

        public IReadOnlyCollection<IContainerModifier> GetModifiers()
        {
            return modifiers.ToList().AsReadOnly();
        }

        private void DependencyCacheInvalidated()
        {
            Engine.Log(LogType.Log, String.Format("Clearing cached value in @{0} due to cache change in modifiers", Path), "engine.internal.container.cache");
            ClearCachedValue();
        }

        private void ClearCachedValue()
        {
            cachedFinalValue = null;
        }

        private void SetCachedValue(object value)
        {
            cachedFinalValue = value;
        }

        public void SetModifiers(List<IContainerModifier> modifiers)
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
            ClearCachedValue();
        }

        internal void DoNotification(string eventName, params object[] args)
        {
            Engine.DoNotification(eventName, Path, args);
        }

        public void Broadcast(string eventName, params object[] args)
        {
            Engine.Log(LogType.Log, "Broadcasting " + eventName + " from #" + Id + ".", "engine.internal.container");
            Engine.Broadcast(eventName, Path, args);
        }

        public void NotifyImmediately(string eventName, IDictionary<string, object> notificationContext = null, params object[] args)
        {
            Engine.Log(LogType.Log, string.Format("[event.{0}] Notifying {1}@{2}.", eventName, Id, Path), "engine.internal.container");
            Engine.NotifyImmediately(eventName, Path, notificationContext, args);
            foreach (var modifier in modifiers)
            {
                modifier.Trigger(Engine, eventName, notificationContext);
            }
        }

        public void NotifyImmediately(string eventName, params object[] args)
        {
            NotifyImmediately(eventName, null, args);
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

        public static implicit operator string(ValueContainer container)
        {
            return container.ValueAsString();
        }

        public static implicit operator bool(ValueContainer valueReference)
        {
            return valueReference.ValueAsBool();
        }

        public override string ToString()
        {
            string valueString = value != null ? value.ToString() : "null";
            return string.Format("Reference #{4} @{0}:{1} (containing {2}{3})", Path, this.Description != "" ? " " + this.Description : "", type, type != "map" && type != "list" ? " " + valueString : "", Id);
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
            return Engine.Subscribe(subscriberDescription, eventName, eventhandlerMethodName, Path, ephemeral);
        }

        private void DoListenerInvocation(ListenerSubscription listener, string eventName, params object[] args)
        {
            Engine.Log(LogType.Log, "Invoking listener " + listener + " from #" + Id + ".", "engine.internal.container");
            Engine.InvokeMethod(listener.MethodName, args);
            /*
            try
            {
            }
            catch (Exception e)
            {
                Engine.Log(LogType.Error, string.Format("Error trying to invoke listener from \"{2}\" for event \"{0}\": {1}.", eventName, e.ToString(), listener.SubscriberDescription), "engine.internal.container");
            }
            */
        }

        public void Unsubscribe(ListenerSubscription subscription)
        {
            Engine.Unsubscribe(subscription);
        }

        public void Unsubscribe(string subscriber, string eventName)
        {
            Engine.Unsubscribe(subscriber, eventName, Path);
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
            else if (value is string)
            {
                try
                {
                    return BigDouble.Parse(value as string);
                }
                catch (Exception ex)
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
            if (value is BigDouble || value is string || value is bool || value is Func<IdleEngine, ValueContainer, object[], object> || value == null || value is IList<ValueContainer>)
            {
                return null;
            }
            if (!(value is IDictionary<string, ValueContainer>))
            {
                throw new InvalidOperationException(string.Format("Failed to coerce a value of type {0} to IDictionary<string, ValueReference>.", value.GetType()));
            }
            return (IDictionary<string, ValueContainer>)value;
        }

        public static IList<ValueContainer> CoerceToList(object value)
        {
            if (value is BigDouble || value is string || value is bool || value is IDictionary<string, ValueContainer> || value == null)
            {
                return null;
            }
            if (!(value is IList<ValueContainer>))
            {
                throw new InvalidOperationException(string.Format("Failed to coerce a value of type {0} to IDictionary<string, ValueReference>.", value.GetType()));
            }
            return (IList<ValueContainer>)value;
        }

        public static object NormalizeValue(object value)
        {
            if (value is BigDouble || value is string || value is bool || value is IDictionary<string, ValueContainer> || value is ValueContainer || value is IList<ValueContainer>)
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
                    Set(snapshot.Value);
                    break;
                case "map":
                    {
                        var deserialized = snapshot.Value as IDictionary<string, Snapshot>;
                        var map = new Dictionary<string, ValueContainer>();
                        foreach (var child in deserialized)
                        {
                            if (child.Value != null)
                            {
                                var container = Engine.GetProperty(child.Value.Path, GetOperationType.GET_OR_CREATE);
                                container.RestoreFromSnapshot(engine, child.Value);
                                map[child.Key] = container;
                            }
                        }
                        Set(map);
                        break;
                    }
                case "list":
                    {
                        var deserialized = snapshot.Value as IList<Snapshot>;
                        IList<ValueContainer> list = new List<ValueContainer>();
                        list = Set(list);
                        foreach (var child in deserialized)
                        {
                            if (child.Value != null)
                            {
                                var nc = Engine.CreateValueContainer();
                                list.Add(nc);
                                nc.RestoreFromSnapshot(engine, child);
                            }
                        }
                        break;
                    }
                case "null":
                    Set((string)null);
                    break;
            }
            InternalSubscribe("#" + Id, EngineReadyEvent.EventName, Id + "OnReady", true);
        }

        // TODO: Move into own file
        [JsonConverter(typeof(Converter))]
        public class Snapshot
        {
            public string Type;
            public string Path;
            public object Value;
            public string UpdateMethod;

            public Snapshot()
            {

            }

            public Snapshot(string path, object value, string updateMethod)
            {
                if (path == null)
                {
                    throw new ArgumentNullException("path");
                }
                Path = path;
                Type = DetermineType(value);
                Value = ConvertValue(Type, value);

                UpdateMethod = updateMethod;
            }

            private object ConvertValue(string type, object value)
            {
                switch (type)
                {
                    case "null":
                        return "null";
                    case "number":
                    case "bool":
                    case "string":
                        return value;
                    case "map":
                        if (value is IDictionary<string, ValueContainer>)
                        {
                            return (value as IDictionary<string, ValueContainer>)
                                .Where(x => !x.Value.Ephemeral)
                                .ToDictionary(x => x.Key, x => x.Value.GetSnapshot());
                        }
                        else if (value is IDictionary<string, Snapshot>)
                        {
                            return (value as IDictionary<string, Snapshot>)
                                .ToDictionary(x => x.Key, x => x.Value);
                        }
                        break;
                    case "list":
                        if (value is IList<ValueContainer>)
                        {
                            return (value as IList<ValueContainer>).Where(x => !x.Ephemeral).Select(x => x.GetSnapshot());
                        }
                        else if (value is IList<Snapshot>)
                        {
                            return (value as IList<Snapshot>);
                        }
                        break;
                }
                throw new InvalidOperationException();
            }

            public class BigDoubleConverter : JsonConverter<BigDouble>
            {
                public override bool CanRead => base.CanRead;

                public override bool CanWrite => base.CanWrite;

                public override BigDouble ReadJson(JsonReader reader, Type objectType, BigDouble existingValue, bool hasExistingValue, JsonSerializer serializer)
                {
                    return BigDouble.Parse(reader.ReadAsString());
                }

                public override void WriteJson(JsonWriter writer, BigDouble value, JsonSerializer serializer)
                {
                    writer.WriteValue(value.ToString());
                }



            }

            public class Converter : JsonConverter<Snapshot>
            {
                public override Snapshot ReadJson(JsonReader reader, Type objectType, Snapshot existingValue, bool hasExistingValue, JsonSerializer serializer)
                {
                    string Type = null;
                    string Path = null;
                    string UpdateMethod = null;
                    object Value = null;
                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonToken.EndObject:
                                return new Snapshot(Path, Value, UpdateMethod);
                            case JsonToken.PropertyName:
                                switch (reader.Value)
                                {
                                    case "Type":
                                        Type = reader.ReadAsString();
                                        break;
                                    case "Path":
                                        Path = reader.ReadAsString();
                                        break;
                                    case "UpdateMethod":
                                        UpdateMethod = reader.ReadAsString();
                                        break;
                                    case "Value":
                                        switch (Type)
                                        {
                                            case "number":
                                                Value = serializer.Deserialize<BigDouble>(reader);
                                                break;
                                            case "bool":
                                                Value = reader.ReadAsBoolean();
                                                break;
                                            case "string":
                                                Value = reader.ReadAsString();
                                                break;
                                            case "list":
                                                reader.Read();
                                                Value = serializer.Deserialize<List<Snapshot>>(reader);
                                                break;
                                            case "map":
                                                reader.Read();
                                                Value = serializer.Deserialize<Dictionary<string, Snapshot>>(reader);
                                                break;
                                        }

                                        break;
                                }
                                break;
                        }
                    }
                    return new Snapshot(Path, Value, UpdateMethod);
                }

                public override void WriteJson(JsonWriter writer, Snapshot value, JsonSerializer serializer)
                {
                    throw new NotImplementedException();
                }

                public override bool CanWrite => false;
            }
        }

        public Snapshot GetSnapshot()
        {
            if (!Ephemeral)
            {
                return new Snapshot(Path, value, updatingMethod);
            }
            return null;
        }

        public class ListenerSubscription // TODO: Move to own file
        {
            public readonly string SubscriptionTarget;
            public readonly string MethodName;
            public readonly string Event;
            public readonly bool Ephemeral;
            public readonly string SubscriberDescription;

            public ListenerSubscription(string subscriptionTarget, string subscriberDescription, string eventName, string methodName, bool ephemeral)
            {
                this.SubscriptionTarget = subscriptionTarget;
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

        public string DataType
        {
            get
            {
                if (cachedFinalValue != null)
                {
                    return ValueContainer.DetermineType(cachedFinalValue);
                }
                else
                {
                    return ValueContainer.DetermineType(value);
                }
            }
        }
    }
}