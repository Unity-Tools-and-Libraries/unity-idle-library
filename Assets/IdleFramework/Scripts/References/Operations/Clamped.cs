using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Clamped : ValueContainer
    {
        private ValueContainer value;
        private ValueContainer min;
        private ValueContainer max;

        private Clamped(ValueContainer value, ValueContainer min, ValueContainer max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            return !BigDouble.Zero.Equals(GetAsNumber(engine));
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return BigDouble.Min(BigDouble.Max(value.GetAsNumber(engine), min.GetAsNumber(engine)), max.GetAsNumber(engine));
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public static Clamped Of(ValueContainer value, ValueContainer min, ValueContainer max)
        {
            return new Clamped(value, min, max);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}