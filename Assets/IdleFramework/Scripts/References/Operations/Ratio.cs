using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Ratio : PropertyReference
    {
        private ValueContainer a;
        private ValueContainer b;

        private Ratio(ValueContainer a, ValueContainer b)
        {
            this.a = a;
            this.b = b;
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            return !BigDouble.Zero.Equals(GetAsNumber(toCheck));
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            var aValue = a.GetAsNumber(engine);
            var bValue = b.GetAsNumber(engine);
            if(bValue.Equals(0))
            {
                return BigDouble.NaN;
            }
            return aValue / bValue;
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return GetAsNumber(engine);
        }

        public static Ratio Of(ValueContainer left, ValueContainer right)
        {
            return new Ratio(left, right);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}