using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class NullValue : LiteralValue
    {
        private NullValue()
        {

        }
        public bool GetAsBoolean(IdleEngine engine)
        {
            return false;
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            return SimplePropertyContainer.EMPTY;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return 0;
        }

        public string GetAsString(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public object RawValue(IdleEngine engine)
        {
            return null;
        }

        public static NullValue INSTANCE = new NullValue();
    }
}