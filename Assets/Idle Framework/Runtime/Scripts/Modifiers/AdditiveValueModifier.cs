using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class AdditiveValueModifier : ValueModifier
    {
        private BigDouble value;
        public AdditiveValueModifier(string id, string description, BigDouble value) : base(id, description, 100)
        {
            this.value = value;
        }

        public override object Apply(object input)
        {
            if(!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            return ((BigDouble)input).Add(value);
        }
    }
}