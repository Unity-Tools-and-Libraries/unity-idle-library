using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class SetValueModifier : ValueModifier
    {
        public SetValueModifier(string id, string description, BigDouble value) : base(id, description, value.ToString(), null, priority: 100)
        {

        }

        public SetValueModifier(string id, string description, bool value) : base(id, description, value.ToString().ToLower(), null, priority: 100)
        {

        }

        public SetValueModifier(string id, string description, string expression, string[] dependencies = null) : base(id, description, expression, dependencies, priority: 100)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return EvaluateCalculationExpression<object>(engine, container);
        }

        public override bool CanApply(object target)
        {
            return true;
        }
    }
}