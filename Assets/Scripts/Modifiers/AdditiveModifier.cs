using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class AdditiveModifier : Modifier
    {
        private Func<IdleEngine, BigDouble> modifierValueCalculator;

        public AdditiveModifier(string id, string description, BigDouble value) : base(id, description, value)
        {
            modifierValueCalculator = e => value;
        }

        public override object Apply(object input)
        {
            if(!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            return ((BigDouble)input).Add(ValueAsNumber());
        }
    }
}