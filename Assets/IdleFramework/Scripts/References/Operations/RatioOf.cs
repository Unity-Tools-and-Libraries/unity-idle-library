using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class RatioOf : PropertyReference
    {
        private PropertyReference a;
        private PropertyReference b;

        public RatioOf(PropertyReference a, PropertyReference b)
        {
            this.a = a;
            this.b = b;
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
    }
}