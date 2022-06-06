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
        public SetValueModifier(string id, string description, BigDouble value) : base(id, description, value.ToString(), true, priority: 100)
        {

        }

        public SetValueModifier(string id, string description, bool value) : base(id, description, value.ToString().ToLower(), true, priority: 100)
        {

        }

        public SetValueModifier(string id, string description, string expression) : base(id, description, expression, priority: 100)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return EvaluateCalculationExpression<object>(engine, container);
        }
    }
}