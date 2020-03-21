using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class BooleanLiteral : LiteralValue
    {
        private readonly bool value;

        public BooleanLiteral(bool value)
        {
            this.value = value;
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            return value;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return value ? 1 : 0;
        }

        public string GetAsString(IdleEngine engine)
        {
            return value.ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return value;
        }
    }
}
