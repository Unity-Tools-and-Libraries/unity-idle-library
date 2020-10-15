using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleFramework
{
    /*
     * Wraps a value. Allows subscribing to watch for value changes
     */
    // TODO: Implement automatic conversion from this to basic type.
    public class ValueReference : Watchable
    {
        private static int nextId = 1;
        private string internalId;
        private object value;
        private ValueReference containingReference;
        private Func<IdleEngine, ValueReference, float, object, object> updater;
        private List<Action<object>> listeners = new List<Action<object>>();
        private bool updatedThisTick = false;

        public string Id => internalId;

        internal ValueReference() : this(null, null) { }
        internal ValueReference(ValueReference containingReference) : this(containingReference, null, null) { }
        internal ValueReference(ValueReference containingReference, object startingValue) : this(containingReference, startingValue, null)
        {
        }

        internal ValueReference(ValueReference containingReference, object startingValue, Func<IdleEngine, ValueReference, float, object, object> updater)
        {
            this.internalId = nextId++.ToString();
            this.containingReference = containingReference;
            this.value = startingValue != null ? startingValue : BigDouble.Zero;
            if (startingValue is ParentNotifyingMap)
            {
                ((ParentNotifyingMap)startingValue).Watch(x => NotifyListeners(value));
            }
            this.updater = updater;
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
                return otherValueReference.value == this.value;
            }
            return false;
        }

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;

        private static bool coerceToBool(object value)
        {
            if (value == null)
            {
                return false;
            }
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
        }

        internal void ClearUpdatedFlag() => updatedThisTick = false;

        private static BigDouble coerceToNumber(object value)
        {
            if (value == null)
            {
                return BigDouble.Zero;
            }
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
            if (value == null)
            {
                return "";
            }
            if (value is string)
            {
                return (string)value;
            }
            return value.ToString();
        }
        private static IDictionary<string, ValueReference> coerceToMap(object value)
        {
            if (value == null || value is BigDouble || value is string || value is bool)
            {
                return null;
            }
            return value as ParentNotifyingMap;
        }

        public void Watch(Action<object> listener)
        {
            listeners.Add(listener);
            listener.Invoke(value);
        }

        private void setInternal(object newValue)
        {
            this.value = newValue;
            NotifyListeners(newValue);
        }

        public void Add(BigDouble amountToAdd)
        {
            if (value is BigDouble)
            {
                Set(amountToAdd + (BigDouble)value);
            }
        }

        public void Set(ValueReference valueReference)
        {
            setInternal(valueReference.ValueAsRaw());
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
            else if (this.value == null)
            {
                valueType = "nothing";
            }
            return string.Format("Reference(containing {0})", valueType);
        }
    }
}