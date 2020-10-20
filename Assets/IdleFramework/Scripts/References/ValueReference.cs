using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleFramework
{
    /*
     * Wraps a value. Allows subscribing to watch for value changes
     */
    // TODO: Implement automatic conversion from this to basic type.
    public class ValueReference : Watchable, CanSnapshot<ValueReference.Snapshot>
    {
        private string internalId;
        private object value;
        private ValueReference containingReference;
        private Func<IdleEngine, ValueReference, float, object, object> updater;
        private List<Action<object>> listeners = new List<Action<object>>();
        private bool updatedThisTick = false;
        private Action<IdleEngine, float, object> postUpdateHook;

        public string Id
        {
            get => internalId;
            set {
                if(internalId != null)
                {
                    throw new InvalidOperationException("Can't reassign internalId");
                }
                internalId = value;
            }
        }
        internal ValueReference() : this(null, null, null, null)
        {

        }

        internal ValueReference(ValueReference containingReference, object startingValue, Action<IdleEngine, float, object> postUpdateHook) : this(containingReference, startingValue, null, postUpdateHook)
        {

        }

        internal ValueReference(ValueReference containingReference, object startingValue, Func<IdleEngine, ValueReference, float, object, object> updater, Action<IdleEngine, float, object> postUpdateHook)
        {
            this.containingReference = containingReference;
            this.value = startingValue != null ? startingValue : BigDouble.Zero;
            if (startingValue is ParentNotifyingMap)
            {
                ((ParentNotifyingMap)startingValue).Watch(x => NotifyListeners(value));
            }
            this.updater = updater;
            this.postUpdateHook = postUpdateHook;
        }

        internal bool UpdatedThisTick => updatedThisTick;

        public bool ValueAsBool()
        {
            return coerceToBool(value);
        }

        public BigDouble ValueAsNumber()
        {
            return coerceToNumber(value);
        }

        public string ValueAsString()
        {
            return coerceToString(value);
        }

        public IDictionary<string, ValueReference> ValueAsMap()
        {
            return coerceToMap(value);

        }

        public override bool Equals(object other)
        {
            if (other is ValueReference)
            {
                ValueReference otherValueReference = other as ValueReference;
                return otherValueReference.value.GetType() == this.value.GetType() &&
                    otherValueReference.value.Equals(this.value);
            }
            return false;
        }

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;

        private static bool coerceToBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            else if (value is ParentNotifyingMap)
            {
                return true;
            }
            else
            {
                return (BigDouble)value != BigDouble.Zero;
            }
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            updatedThisTick = true;
            if (this.updater != null)
            {
                value = this.updater.Invoke(engine, containingReference, deltaTime, value);
                NotifyListeners(value);
            }
            if (this.value is IDictionary<string, ValueReference>)
            {
                foreach (var child in ((IDictionary<string, ValueReference>)this.value))
                {
                    child.Value.Update(engine, deltaTime);
                }
            }
            postUpdateHook?.Invoke(engine, deltaTime, value);
        }

        internal void ClearUpdatedFlag() => updatedThisTick = false;

        private static BigDouble coerceToNumber(object value)
        {
            if (value is bool)
            {
                return (bool)value ? BigDouble.One : BigDouble.Zero;
            }
            else if (value is ParentNotifyingMap)
            {
                return BigDouble.Zero;
            }
            else
            {
                return (BigDouble)value;
            }
        }
        private static string coerceToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            return value.ToString();
        }
        private static IDictionary<string, ValueReference> coerceToMap(object value)
        {
            if (value is BigDouble || value is string || value is bool)
            {
                return null;
            }
            if(!(value is IDictionary<string, ValueReference>))
            {
                throw new InvalidOperationException(string.Format("Failed to coerce a value of type {0} to IDictionary<string, ValueReference>.", value.GetType()));
            }
            return value as IDictionary<string, ValueReference>;
        }

        public void Watch(Action<object> listener)
        {
            listeners.Add(listener);
            listener.Invoke(value);
        }

        private void setInternal(object newValue)
        {
            this.value = newValue != null ? newValue : BigDouble.Zero;
            NotifyListeners(newValue);
        }

        public void Set(BigDouble newValue) => setInternal(newValue);

        public void Set(string newValue) => setInternal(newValue);

        public void Set(bool newValue) => setInternal(newValue);

        public void Set(Dictionary<string, ValueReference> newValue) => setInternal(newValue);

        private void NotifyListeners(object newValue)
        {
            foreach (var listener in listeners.ToArray())
            {
                listener.Invoke(newValue);
            }
        }

        public object ValueAsRaw()
        {
            return value;
        }

        public static implicit operator BigDouble(ValueReference valueReference)
        {
            return valueReference.ValueAsNumber();
        }

        public static implicit operator string(ValueReference valueReference)
        {
            return valueReference.ValueAsString();
        }

        public static implicit operator bool(ValueReference valueReference)
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
            else if (this.value is IDictionary<string, ValueReference>)
            {
                valueType = "map";
            }
            return string.Format("Reference(containing {0})", valueType);
        }

        public Snapshot GetSnapshot()
        {
            return new Snapshot(internalId, value);
        }

        public void RestoreFromSnapshot(IdleEngine engine, Snapshot snapshot)
        {
            if(internalId != snapshot.internalId)
            {
                throw new InvalidOperationException("The internalId of the snapshot and this reference don't match");
            }
            if (snapshot.value is IDictionary<string, ValueReference.Snapshot>)
            {
                setInternal(((IDictionary<string, ValueReference.Snapshot>)snapshot.value)
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

        public class Snapshot
        {
            public readonly string internalId;
            public readonly object value;
            public Snapshot(string internalId, object value)
            {
                this.internalId = internalId;
                if (value is ParentNotifyingMap)
                {
                    this.value = ((IDictionary<string, ValueReference>)value).ToDictionary(e => e.Key,
                        e => e.Value.GetSnapshot()
                    );
                } else {
                    this.value = value;
                }
            }

            public override string ToString()
            {
                return string.Format(@"{{{0}, {1}}}", internalId, value.ToString());
            }

            public override bool Equals(object obj)
            {
                if(!(obj is Snapshot))
                {
                    return false;
                }
                Snapshot other = obj as Snapshot;
                bool sameId = String.Equals(other.internalId, internalId);
                bool valueSameType = value.GetType() == other.value.GetType();
                bool sameValue = value is IDictionary<string, ValueReference.Snapshot> ?
                    (value as IDictionary<string, ValueReference.Snapshot>).SequenceEqual((other.value as IDictionary<string, ValueReference.Snapshot>)) :
                    object.Equals(value, other.value);

                return sameId && valueSameType && sameValue;
            }

            public override int GetHashCode()
            {
                return internalId.GetHashCode() ^ value.GetHashCode();
            }
        }
    }
}