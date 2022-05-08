using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework
{
    /*
     * Wraps a value.
     * 
     * Can watch the container to become notified of changes.
     */
    // TODO: Implement automatic conversion from this to basic type.
    public class ValueContainer : CanSnapshot<ValueContainer.Snapshot>, EventSource
    {
        public delegate object UpdatingMethod(IdleEngine engine, float timeSinceLastUpdate, object previousValue, List<ValueModifier> modifiersList);

        public static BigDouble DEFAULT_VALUE = BigDouble.Zero;
        // The unique id of this container.
        private string internalId;
        // The value held by this container.
        private object value;
        // Method which updates the contained value each tick, if specified.
        private UpdatingMethod updater;
        // Listeners for events on this container.
        private Dictionary<string, List<Action<object>>> eventListeners = new Dictionary<string, List<Action<object>>>();

        private IdleEngine engine;

        private bool updatedThisTick = false;
        private Action<IdleEngine, float, object> postUpdateHook;
        private List<ValueModifier> modifiers;

        public string Id
        {
            get => internalId;
            set
            {
                if (internalId != null)
                {
                    throw new InvalidOperationException("Can't reassign internalId");
                }
                internalId = value;
            }
        }

        public ValueContainer(IdleEngine engine) : this(engine, null as object, null) { }

        internal ValueContainer(IdleEngine engine, object startingValue, List<ValueModifier> modifiers = null, UpdatingMethod updater = null, Action<IdleEngine, float, object> postUpdateHook = null) : this(engine, startingValue, updater, postUpdateHook, modifiers)
        {

        }

        internal ValueContainer(IdleEngine engine, string startingValue, List<ValueModifier> modifiers = null, UpdatingMethod updater = null, Action<IdleEngine, float, object> postUpdateHook = null) : this(engine, startingValue, updater, postUpdateHook, modifiers)
        {

        }

        internal ValueContainer(IdleEngine engine, BigDouble startingValue, List<ValueModifier> modifiers = null, UpdatingMethod updater = null, Action<IdleEngine, float, object> postUpdateHook = null) : this(engine, startingValue, updater, postUpdateHook, modifiers)
        {

        }

        internal ValueContainer(IdleEngine engine, bool startingValue, List<ValueModifier> modifiers = null, UpdatingMethod updater = null, Action<IdleEngine, float, object> postUpdateHook = null) : this(engine, startingValue, updater, postUpdateHook, modifiers)
        {

        }

        internal ValueContainer(IdleEngine engine, IDictionary<string, ValueContainer> startingValue, List<ValueModifier> modifiers = null, UpdatingMethod updater = null, Action<IdleEngine, float, object> postUpdateHook = null) : this(engine, startingValue, updater, postUpdateHook, modifiers)
        {

        }

        internal ValueContainer(IdleEngine engine, object startingValue, UpdatingMethod updater, Action<IdleEngine, float, object> postUpdateHook, List<ValueModifier> startingModifiers)
        {
            this.engine = engine;
            this.modifiers = startingModifiers != null ? startingModifiers : new List<ValueModifier>();
            if (startingValue is IDictionary<string, ValueContainer>)
            {
                Debug.Log("Wrapping dictionary in parent notifying dictionary");
                ParentNotifyingDictionary notifyingDictionary = !(startingValue is ParentNotifyingDictionary) ? new ParentNotifyingDictionary(startingValue as IDictionary<string, ValueContainer>) : startingValue as ParentNotifyingDictionary;
                notifyingDictionary.SetParent(this);
                startingValue = notifyingDictionary;
            }
            this.value = applyModifiers(startingValue != null ? startingValue : BigDouble.Zero);

            this.updater = updater;
            this.postUpdateHook = postUpdateHook;
        }

        private object applyModifiers(object v)
        {
            if (v is BigDouble)
            {
                return applyModifiersToBigDouble((BigDouble)v);
            }
            return v;
        }

        private BigDouble applyModifiersToBigDouble(BigDouble value)
        {
            return modifiers.OrderByDescending(x => x.priority).Aggregate(value, (previousValue, nextModifier) => (BigDouble)nextModifier.Apply(previousValue));
        }

        internal bool UpdatedThisTick => updatedThisTick;

        public Dictionary<string, List<Action<object>>> EventListeners => eventListeners;

        public bool ValueAsBool()
        {
            AssertIsRegistered();
            return CoerceToBool(value);
        }

        private void AssertIsRegistered()
        {
            if (Id == null)
            {
                throw new InvalidOperationException("ValueContainer must be registered via Register");
            }
        }

        public BigDouble ValueAsNumber()
        {
            AssertIsRegistered();
            return CoerceToNumber(value);
        }

        public string ValueAsString()
        {
            AssertIsRegistered();
            return CoerceToString(value);
        }

        public IDictionary<string, ValueContainer> ValueAsMap()
        {
            AssertIsRegistered();
            return CoerceToMap(value);
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
            if (updatedThisTick)
            {
                return;
            }
            updatedThisTick = true;
            if (this.updater != null)
            {
                var updateOut = this.updater.Invoke(engine, deltaTime, value, this.modifiers);
                bool outputIsValid = false;
                if (updateOut == null)
                {
                    throw new InvalidOperationException("Update method cannot return null!");
                }
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
                if (updateOut is bool || updateOut is string || updateOut is ParentNotifyingDictionary || updateOut is BigDouble)
                {
                    outputIsValid = true;
                }
                if (outputIsValid)
                {
                    this.value = applyModifiers(updateOut);
                    Notify(Events.VALUE_CHANGED, this.value);
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
            postUpdateHook?.Invoke(engine, deltaTime, value);
        }

        internal void ClearUpdatedFlag() => updatedThisTick = false;

        private void setInternal(object newValue)
        {
            this.value = applyModifiers(newValue != null ? newValue : BigDouble.Zero);
            Notify(Events.VALUE_CHANGED, newValue);
        }

        public void Set(BigDouble newValue)
        {
            AssertCanSet();
            setInternal(newValue);
        }

        public void Set(string newValue)
        {
            AssertCanSet();
            setInternal(newValue);
        }

        public void Set(bool newValue)
        {
            AssertCanSet();
            setInternal(newValue);
        }

        public void Set(IDictionary<string, ValueContainer> newValue)
        {
            if (!(newValue is ParentNotifyingDictionary))
            {
                Debug.Log("Wrapping dictionary in parent notifying dictionary");
                newValue = new ParentNotifyingDictionary(newValue as IDictionary<string, ValueContainer>, this);
            }
            foreach(var child in newValue.Values)
            {

            }
            AssertCanSet();
            setInternal(newValue);
        }

        public IReadOnlyCollection<ValueModifier> GetModifiers()
        {
            return modifiers.AsReadOnly();
        }

        public void Notify(string eventName, object newValue)
        {
            List<Action<object>> listeners;
            if (eventListeners.TryGetValue(eventName, out listeners))
            {
                foreach (var listener in listeners.ToArray())
                {
                    listener.Invoke(newValue);
                }
            }
        }

        public void SetUpdater(UpdatingMethod p)
        {
            updater = p;
        }

        public object ValueAsRaw()
        {
            return value;
        }

        public static implicit operator BigDouble(ValueContainer valueReference)
        {
            return valueReference.ValueAsNumber();
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
            }
            else if (this.value is IDictionary<string, ValueContainer>)
            {
                valueType = "map";
            }
            return string.Format("Reference #{0}(containing {1})", Id, valueType);
        }

        public Snapshot GetSnapshot()
        {
            return new Snapshot(internalId, this);
        }

        public void RestoreFromSnapshot(IdleEngine engine, Snapshot snapshot, ValueContainer parent = null)
        {
            if (internalId != snapshot.internalId)
            {
                throw new InvalidOperationException("The internalId of the snapshot and this reference don't match");
            }
            if (snapshot.value is IDictionary<string, ValueContainer.Snapshot>)
            {
                var restoredValue = ((IDictionary<string, ValueContainer.Snapshot>)snapshot.value)
                    .ToDictionary(e => e.Key, e =>
                    {
                        var existingReference = engine.GetReferenceById(e.Value.internalId);
                        existingReference.RestoreFromSnapshot(engine, e.Value);
                        return existingReference;
                    });
                setInternal(new ParentNotifyingDictionary(restoredValue, parent));
            }
            else
            {
                setInternal(snapshot.value);
            }
        }

        public void Subscribe(string eventName, Action<object> listener)
        {
            Debug.Log("Subscribing to " + eventName + " event");
            List<Action<object>> listeners;
            if (!eventListeners.TryGetValue(eventName, out listeners))
            {
                listeners = new List<Action<object>>();
                eventListeners[eventName] = listeners;
            }
            listeners.Add(listener);
            if(eventName == Events.VALUE_CHANGED)
            {
                listener.Invoke(value);
            }
        }

        public void Unsubscribe(string eventName, Action<object> listener)
        {
            List<Action<object>> listeners;
            if (eventListeners.TryGetValue(eventName, out listeners))
            {
                var removed = listeners.Remove(listener);
                Debug.Log(removed);
            }
        }

        private void AssertCanSet()
        {
            if (updater != null)
            {
                throw new InvalidOperationException("Cannot call set on a value reference with an update method.");
            }
        }

        public class Snapshot
        {
            public readonly string internalId;
            public readonly object value;
            public Snapshot(string internalId, ValueContainer value)
            {
                this.internalId = internalId;
                if (value.ValueAsMap() != null)
                {
                    this.value = value.ValueAsMap().ToDictionary(e => e.Key,
                        e => e.Value.GetSnapshot()
                    );
                }
                else
                {
                    this.value = value.ValueAsRaw();
                }
            }

            public Snapshot(string internalId, IDictionary<string, Snapshot> snapshots)
            {
                this.internalId = internalId;
                value = snapshots;
            }

            public override string ToString()
            {
                return string.Format(@"{{{0}, {1}}}", internalId, value.ToString());
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Snapshot))
                {
                    return false;
                }
                Snapshot other = obj as Snapshot;
                bool sameId = String.Equals(other.internalId, internalId);
                bool valueSameType = value.GetType() == other.value.GetType();
                bool sameValue = value is IDictionary<string, ValueContainer.Snapshot> ?
                    (value as IDictionary<string, ValueContainer.Snapshot>).SequenceEqual((other.value as IDictionary<string, ValueContainer.Snapshot>)) :
                    object.Equals(value, other.value);

                return sameId && valueSameType && sameValue;
            }

            public override int GetHashCode()
            {
                return internalId.GetHashCode() ^ (value != null ? value.GetHashCode() : 0);
            }
        }

        public static class Events
        {
            public static readonly string VALUE_CHANGED = "valueChanged";
        }

        private static bool CoerceToBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            else if (value is IDictionary<string, ValueContainer>)
            {
                return true;
            }
            else
            {
                return (BigDouble)value != BigDouble.Zero;
            }
        }

        private static BigDouble CoerceToNumber(object value)
        {
            if (value is bool)
            {
                return (bool)value ? BigDouble.One : BigDouble.Zero;
            }
            else if (value is IDictionary<string, ValueContainer>)
            {
                return BigDouble.Zero;
            }
            else
            {
                return (BigDouble)value;
            }
        }
        private static string CoerceToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            return value.ToString();
        }
        private static IDictionary<string, ValueContainer> CoerceToMap(object value)
        {
            if (value is BigDouble || value is string || value is bool)
            {
                return null;
            }
            if (!(value is IDictionary<string, ValueContainer>))
            {
                throw new InvalidOperationException(string.Format("Failed to coerce a value of type {0} to IDictionary<string, ValueReference>.", value.GetType()));
            }
            //Debug.Log("Coercing to map " + value != null ? value.ToString() : null);
            return (ParentNotifyingDictionary)value;
        }
    }
}