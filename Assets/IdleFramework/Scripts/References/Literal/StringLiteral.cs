using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class StringLiteral : LiteralValue
    {
        private readonly string value;

        public StringLiteral(string value)
        {
            this.value = value;
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            bool boolValue ;
            bool.TryParse(value, out boolValue);
            return boolValue;
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return BigDouble.Parse(value);
        }

        public string GetAsString(IdleEngine engine)
        {
            return value;
        }

        public object RawValue(IdleEngine engine)
        {
            return value;
        }
    }
}