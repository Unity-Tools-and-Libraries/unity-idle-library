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

        public BigDouble Get(IdleEngine engine)
        {
            var aValue = a.Get(engine);
            var bValue = b.Get(engine);
            if(bValue.Equals(0))
            {
                return BigDouble.NaN;
            }
            return aValue / bValue;
        }
    }
}