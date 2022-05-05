using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class FixedValueModifier : ValueModifier
    {
        private object fixedValue;

        public FixedValueModifier(string id, string description, string fixedValue) : base(id, description, int.MinValue)
        {
            this.fixedValue = fixedValue;
        }

        public FixedValueModifier(string id, string description, BigDouble fixedValue) : base(id, description, int.MinValue)
        {
            this.fixedValue = fixedValue;
        }

        public FixedValueModifier(string id, string description, bool fixedValue) : base(id, description, int.MinValue)
        {
            this.fixedValue = fixedValue;
        }

        public override object Apply(object input)
        {
            return fixedValue;
        }
    }
}