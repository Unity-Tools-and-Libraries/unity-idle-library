using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class NumberLiteral : LiteralValue
    {
        private readonly BigDouble value;

        public NumberLiteral(BigDouble value)
        {
            this.value = value;
        }

        public BigDouble Value => value;

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            return !BigDouble.Zero.Equals(value);
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return value;
        }

        public string GetAsString(IdleEngine engine)
        {
            return value.ToString();
        }

        public override string ToString()
        {
            return string.Format("Literal {0}", value);
        }

        public object RawValue(IdleEngine engine)
        {
            return value;
        }
    }
}