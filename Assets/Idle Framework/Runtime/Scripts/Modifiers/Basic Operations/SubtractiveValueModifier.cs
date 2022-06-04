using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class SubtractiveValueModifier : ValueModifier
    {
        public SubtractiveValueModifier(string id, string description, BigDouble value) : base(id, description, value.ToString(), true, priority: ValueModifier.DefaultPriorities.ADDITION)
        {

        }

        public SubtractiveValueModifier(string id, string description, string expression) : base(id, description, expression, priority: ValueModifier.DefaultPriorities.ADDITION)
        {

        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression(engine);
            return ((BigDouble)input).Subtract(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble)expressionResult);
        }
    }
}