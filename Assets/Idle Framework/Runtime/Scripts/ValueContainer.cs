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
using static io.github.thisisnozaku.idle.framework.IdleEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

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
                if(ValueAsMap() != null)
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
                } else
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

        private List<ContainerModifier> modifiers;

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

        internal ValueContainer(IdleEngine engine, string id, string startingValue, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, BigDouble startingValue, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, bool startingValue, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, IDictionary<string, ValueContainer> startingValue, string description = "", string path = null, List<ContainerModifier> modifiers = null, string updaterMethodName = null, string interceptorMethod = null) : this(engine, id, startingValue, description, path, updaterMethodName, modifiers, interceptorMethod)
        {

        }

        internal ValueContainer(IdleEngine engine, string id, object startingValue, string description, string path, string updater, List<ContainerModifier> startingModifiers, string interceptorMethod)
        {
            this.Id = id;
            this.Path = path;
            this.Engine = engine;
            this.modifiers = startingModifiers != null ? startingModifiers : new List<ContainerModifier>();
            if (startingValue is IDictionary<string, ValueContainer>)
            {
                Debug.Log("Wrapping dictionary in parent notifying dictionary");
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
            InternalSubscribe("#" + Id, IdleEngine.Events.ENGINE_READY, Id + "OnReady", true);
        }

        private object OnReady(IdleEngine engine, ValueContainer container, object ev)
        {
            NotifyImmediately(ValueContainer.Events.VALUE_CHANGED, new ValueChangedEvent(Path, null, this.value, this));
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
                return applyModifiersToBigDouble((BigDouble)v);
            }
            else if (v is bool)
            {
                return applyModifiersToBool((bool)v);
            } else if (v is string)
            {
                return applyModifiersToString(v as string);
            }
            return v;
        }

        private BigDouble applyModifiersToBigDouble(BigDouble originalValue)
        {
            return modifiers.OrderByDescending(x => x.priority).Aggregate(originalValue, (previousValue, nextModifier) => (BigDouble)nextModifier.Apply(Engine, this, previousValue));
        }

        private bool applyModifiersToBool(bool originalValue)
        {
            return modifiers.OrderByDescending(x => x.priority).Aggregate(originalValue, (previousValue, nextModifier) => (bool)nextModifier.Apply(Engine, this, previousValue));
        }

        private string applyModifiersToString(string originalValue)
        {
            return modifiers.OrderByDescending(x => x.priority).Aggregate(originalValue, (previousValue, nextModifier) => (string)nextModifier.Apply(Engine, this, previousValue));
        }

        public bool ValueAsBool()
        {
            AssertIsRegistered();
            return applyModifiersToBool(CoerceToBool(value));
        }

        private void AssertIsRegistered()
        {
            if (Id == null || Path == null)
            {
                Debug.LogError("ValueContainer is not ready to be used; it must be assigned to a global property in the engine or a descendent of one before use.");
            }
        }

        public BigDouble ValueAsNumber()
        {
            AssertIsRegistered();
            return (BigDouble)applyModifiers(CoerceToNumber(value));
        }

        public string ValueAsString()
        {
            AssertIsRegistered();
            return (string)applyModifiers(CoerceToString(value));
        }

        public IDictionary<string, ValueContainer> ValueAsMap()
        {
            AssertIsRegistered();
            return (IDictionary<string, ValueContainer>)applyModifiers(CoerceToMap(value));
        }

        public object ValueAsRaw()
        {
            return applyModifiers(value);
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
                var updateOut = engine.InvokeMethod(this.updatingMethod, this, new ValueContainerWillUpdateEvent(this, deltaTime, this.value));
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
            NotifyImmediately(Events.UPDATED, new ValueContainerUpdatedEvent(this));
        }

        private object setInternal(object newValue)
        {
            var old = this.value;
            this.value = this.interceptorMethod != null ? Engine.InvokeMethod(this.interceptorMethod, this, new ValueContainerValueWillSet(this, this.value, newValue)) : newValue;
            NotifyChange(old, this.value);
            return this.value;
        }

        private void NotifyChange(object oldValue, object newValue)
        {
            NotifyImmediately(Events.VALUE_CHANGED, new ValueChangedEvent(Path, oldValue, newValue, this));
            var splitPath = Path.Split('.');
            if (Parent != null)
            {
                var childChangeEvent = new ValueChangedEvent(Path, oldValue, newValue, this);
                childChangeEvent.PreventBubbling = false;
                Parent.NotifyImmediately(Events.CHILD_VALUE_CHANGED, childChangeEvent);
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
                Debug.Log("Wrapping dictionary in parent notifying dictionary");
                newValue = new ParentNotifyingDictionary(this, newValue as IDictionary<string, ValueContainer>);
            }
            AssertCanSet();
            setInternal(newValue);
            return this;
        }

        public IReadOnlyCollection<ContainerModifier> GetModifiers()
        {
            return modifiers.AsReadOnly();
        }

        internal void SetModifiers(List<ContainerModifier> modifiers)
        {
            this.modifiers = modifiers != null ? modifiers : new List<ContainerModifier>();
        }

        internal void DoNotification(string eventName, IdleEngineEvent ev)
        {
            ISet<ListenerSubscription> listeners;
            if (this.listeners.TryGetValue(eventName, out listeners))
            {
                listeners = new HashSet<ListenerSubscription>(listeners); // FIXM: Ew gross yuck.
                foreach (var listener in listeners)
                {
                    DoListenerInvocation(listener, ev, eventName);
                }
            }
        }

        internal void Broadcast(string eventName, IdleEngineEvent ev)
        {
            Debug.Log("Broadcasting " + eventName + " from #" + Id + ".");
            ev.PreventBubbling = true;
            DoNotification(eventName, ev);
            if (ValueAsMap() != null)
            {
                foreach (var child in ValueAsMap())
                {
                    child.Value.Broadcast(eventName, ev);
                }
            }
        }

        public void NotifyImmediately(string eventName, IdleEngineEvent ev)
        {
            if (Engine.IsReady)
            {
                Debug.Log(string.Format("[event.{0}] Notifying {1}@{2}.", eventName, Id, Path));
                AssertIsRegistered();
                DoNotification(eventName, ev);
                foreach(var mod in modifiers)
                {
                    mod.Trigger(Engine, eventName);
                }
                Engine.BubbleEvent(eventName, ev, this);
            } else
            {
                Debug.LogWarning("Start() has not been called on the engine.");
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

        public ValueContainer AddModifier(ContainerModifier modifier)
        {
            if (!this.modifiers.Any(m => m.Id == modifier.Id))
            {
                var old = this.ValueAsRaw();
                this.modifiers.Add(modifier);
                modifier.OnAdd(Engine, this);
                NotifyChange(old, this.ValueAsRaw());
            }
            return this;
        }

        public ValueContainer RemoveModifier(ContainerModifier modifier)
        {
            var old = this.ValueAsRaw();
            bool removed = this.modifiers.Remove(modifier);
            if(removed)
            {
                modifier.OnRemoval(Engine, this);
            }
            NotifyChange(old, this.ValueAsRaw());
            return this;
        }

        public ListenerSubscription Subscribe(string subscriberDescription, string eventName, string eventHandlerName, bool ephemeral = false)
        {
            var listener = InternalSubscribe(subscriberDescription, eventName, eventHandlerName, ephemeral);
            if (eventName == ValueContainer.Events.VALUE_CHANGED || eventName == IdleEngine.Events.ENGINE_READY)
            {
                if (Engine.IsReady)
                {
                    DoListenerInvocation(listener, new ValueChangedEvent(Path, this.value, this.value, this), eventName);
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

        private void DoListenerInvocation(ListenerSubscription listener, IdleEngineEvent ev, string eventName)
        {
            try
            {
                Debug.Log("Invoking listener " + listener + " from #" + Id + ".");
                Engine.InvokeMethod(listener.MethodName, this, ev);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error trying to invoke listener from \"{2}\" for event \"{0}\": {1}.", eventName, e.ToString(), listener.SubscriberDescription));
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
                    .FirstOrDefault(null) ;
                if (subscription != null)
                {
                    listeners.Remove(subscription);
                }
            }
        }

        private void AssertCanSet()
        {

        }

        public static class Events
        {
            public const string VALUE_CHANGED = "valueChanged";
            public const string UPDATED = "UPDATED";
            public const string CHILD_VALUE_CHANGED= "childValueChanged";
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
            } else if(value is string)
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
            //Debug.Log("Coercing to map " + value != null ? value.ToString() : null);
            return (IDictionary<string, ValueContainer>)value;
        }

        public static object NormalizeValue(object value)
        {
            if(value is BigDouble || value is string || value is bool || value is IDictionary<string, ValueContainer> || value is ValueContainer)
            {
                return value;
            }
            if(value is int)
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
            if(value is decimal)
            {
                return BigDouble.Parse(value.ToString());
            }
            if(value is null)
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
            InternalSubscribe("#" + Id, IdleEngine.Events.ENGINE_READY, Id + "OnReady", true);
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
    
        
    }
}