using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class ListLiteral : LiteralValue
    {
        private ValueContainer[] values;

        public ListLiteral(params ValueContainer[] values)
        {
            this.values = values;
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
            return null;
        }

        public object RawValue(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}