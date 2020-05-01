using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class StringLiteral : StringContainer
    {
        private readonly string value;

        public StringLiteral(string value)
        {
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is StringLiteral literal &&
                   value == literal.value;
        }

        public string Get(IdleEngine engine)
        {
            return value;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(value);
        }

        public override string ToString()
        {
            return string.Format("StringLiteral(\"{0}\")", value);
        }

    }
}