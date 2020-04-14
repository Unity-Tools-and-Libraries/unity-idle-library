using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Floor : NumberContainer
    {
        private NumberContainer value;

        public Floor(NumberContainer value)
        {
            this.value = value;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return BigDouble.Floor(value.Get(engine));
        }

        public static Floor Of(NumberContainer value)
        {
            return new Floor(value);
        }
    }
}