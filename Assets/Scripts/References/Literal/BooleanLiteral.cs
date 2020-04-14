using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class BooleanLiteral : BooleanContainer
    {
        private readonly bool value;

        public BooleanLiteral(bool value)
        {
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is BooleanLiteral literal &&
                   value == literal.value;
        }

        public bool Get(IdleEngine engine)
        {
            return value;
        }
    }
}
