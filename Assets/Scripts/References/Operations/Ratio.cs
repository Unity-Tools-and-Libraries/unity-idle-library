using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Ratio : NumberContainer
    {
        private readonly NumberContainer a;
        private readonly NumberContainer b;

        private Ratio(NumberContainer a, NumberContainer b)
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

        public static NumberContainer Of(NumberContainer numerator, NumberContainer denominator )
        {
            return new Ratio(numerator, denominator);
        }
    }
}