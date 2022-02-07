using BreakInfinity;
using IdleFramework.Events;
using IdleFramework.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleFramework
{
    /*
     * Wraps a value.
     * 
     * Can watch the container to become notified of changes.
     */
    // TODO: Implement automatic conversion from this to basic type.
    public class ValueContainer : CanSnapshot<ValueContainer.Snapshot>, EventSource
    {
        // The unique id of this container.
        private string internalId;
        // The value contained by this container.
        private object value;
        private ValueContainer parentReference;
        // Method which updates the contained value each tick, if specified.
        private Func<IdleEngine, float, object, ValueContainer, object> updater;
        // Listeners for events on this container.
        private Dictionary<string, List<Action<object>>> eventListeners = new Dictionary<string, List<Action<object>>>();
        private bool updatedThisTick = false;
        private Action<IdleEngine, float, object> postUpdateHook;
        private List<Modifier> modifiers;

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

        internal ValueContainer() : this(null, null, null, null, new List<Modifier>())
        {

        }

        internal ValueContainer(ValueContainer containingReference, object startingValue, Action<IdleEngine, float, object> postUpdateHook) : this(containingReference, startingValue, null, postUpdateHook, new List<Modifier>())
        {

        }

        internal ValueContainer(ValueContainer containingReference, object startingValue, Func<IdleEngine, float, object, ValueContainer, object> updater, Action<IdleEngine, float, object> postUpdateHook, List<Modifier> startingModifiers)
        {
            this.modifiers = startingModifiers;
            this.parentReference = containingReference;
            this.value = applyModifiers(startingValue != null ? startingValue : BigDouble.Zero);
            if (startingValue is ParentNotifyingDictionary)
            {
                ((ParentNotifyingDictionary)startingValue).Watch(x => NotifyListeners(Events.VALUE_CHANGED, value));
            }
            this.updater = updater;
            this.postUpdateHook = postUpdateHook;
        }

        private object applyModifiers(object v)
        {
            if(v is BigDouble)
            {
                return applyModifiersToBigDouble((BigDouble)v);
            }
            return v;
        }

        private BigDouble applyModifiersToBigDouble(BigDouble value)
        {
            var additiveModifiers = modifiers.Where(m => m is AdditiveModifier);
            var multiplicativeModifiers = modifiers.Where(m => m is MultiplicativeModifier);
            foreach(var additiveModifier in additiveModifiers)
            {
                value = (BigDouble)additiveModifier.Apply(value);
            }
            foreach(var multiplicativeModifier in multiplicativeModifiers)
            {
                value = (BigDouble)multiplicativeModifier.Apply(value);
            }
            return value;
        }

        internal bool UpdatedThisTick => updatedThisTick;

        public Dictionary<string, List<Action<object>>> EventListeners => eventListeners;

        public bool ValueAsBool()
        {
            return IdleEngine.CoerceToBool(value);
        }

        public BigDouble ValueAsNumber()
        {
            return IdleEngine.CoerceToNumber(value);
        }

        public string ValueAsString()
        {
            return IdleEngine.CoerceToString(value);
        }

        public IDictionary<string, ValueContainer> ValueAsMap()
        {
            return IdleEngine.CoerceToMap(value);

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

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;

        public void Update(IdleEngine engine, float deltaTime)
        {
            if(updatedThisTick)
            {
                return;
            }
            updatedThisTick = true;
            if (this.updater != null)
            {
                var updateOut = this.updater.Invoke(engine, deltaTime, value, this.parentReference);
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
                    this.value= applyModifiers(updateOut);
                    NotifyListeners(Events.VALUE_CHANGED, this.value);
                } else
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

        public void Watch(Action<object> listener)
        {
            Subscribe(Events.VALUE_CHANGED, listener);
            listener.Invoke(value);
        }

        private void setInternal(object newValue)
        {
            this.value = applyModifiers(newValue != null ? newValue : BigDouble.Zero);
            NotifyListeners(Events.VALUE_CHANGED, newValue);
        }

        public void Set(BigDouble newValue) {
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

        public void Set(Dictionary<string, ValueContainer> newValue)
        {
            AssertCanSet();
            setInternal(newValue);
        }

        public IReadOnlyCollection<Modifier> GetModifiers()
        {
            return modifiers.AsReadOnly();
        }

        private void NotifyListeners(string eventName, object newValue)
        {
            List<Action<Object>> listeners;
            if (eventListeners.TryGetValue(eventName, out listeners))
            {
                foreach (var listener in listeners.ToArray())
                {
                    listener.Invoke(newValue);
                }
            }
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
            return new Snapshot(internalId, value);
        }

        public void RestoreFromSnapshot(IdleEngine engine, Snapshot snapshot)
        {
            if (internalId != snapshot.internalId)
            {
                throw new InvalidOperationException("The internalId of the snapshot and this reference don't match");
            }
            if (snapshot.value is IDictionary<string, ValueContainer.Snapshot>)
            {
                setInternal(((IDictionary<string, ValueContainer.Snapshot>)snapshot.value)
                    .ToDictionary(e => e.Key, e =>
                    {
                        var existingReference = engine.GetReferenceById(e.Value.internalId);
                        existingReference.RestoreFromSnapshot(engine, e.Value);
                        return existingReference;
                    }));
            }
            else
            {
                setInternal(snapshot.value);
            }
        }

        public void Subscribe(string eventName, Action<object> listener)
        {
            List<Action<object>> listeners;
            if (!eventListeners.TryGetValue(eventName, out listeners))
            {
                listeners = new List<Action<object>>();
                eventListeners[eventName] = listeners;
            }
            listeners.Add(listener);
        }

        private void AssertCanSet()
        {
            if(updater != null)
            {
                throw new InvalidOperationException("Cannot call set on a value reference with an update method.");
            }
        }

        public class Snapshot
        {
            public readonly string internalId;
            public readonly object value;
            public Snapshot(string internalId, object value)
            {
                this.internalId = internalId;
                if (value is ParentNotifyingDictionary)
                {
                    this.value = ((IDictionary<string, ValueContainer>)value).ToDictionary(e => e.Key,
                        e => e.Value.GetSnapshot()
                    );
                }
                else
                {
                    this.value = value;
                }
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
                return internalId.GetHashCode() ^ value.GetHashCode();
            }
        }

        public static class Events
        {
            public static readonly string VALUE_CHANGED = "valueChanged";
        }
    }
}