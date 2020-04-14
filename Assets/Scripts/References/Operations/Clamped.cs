using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Clamped : NumberContainer
    {
        private NumberContainer value;
        private NumberContainer min;
        private NumberContainer max;

        private Clamped(NumberContainer value, NumberContainer min, NumberContainer max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public BigDouble Get(IdleEngine engine)
        {
            BigDouble minValue = min.Get(engine);
            BigDouble maxValue = max.Get(engine);
            BigDouble actual = value.Get(engine);
            if (minValue > actual)
            {
                return minValue;
            }
            if (maxValue < actual)
            {
                return maxValue;
            }
            return actual;
        }

        public static Clamped Of(NumberContainer value, NumberContainer min, NumberContainer max)
        {
            return new Clamped(value, min, max);
        }
    }
}