using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Floor : ValueContainer
    {
        private ValueContainer value;

        public Floor(ValueContainer value)
        {
            this.value = value;
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            return BigDouble.Zero.Equals(GetAsNumber(engine));
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return BigDouble.Floor(value.GetAsNumber(engine));
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return value;
        }

        public static Floor Of(ValueContainer value)
        {
            return new Floor(value);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}