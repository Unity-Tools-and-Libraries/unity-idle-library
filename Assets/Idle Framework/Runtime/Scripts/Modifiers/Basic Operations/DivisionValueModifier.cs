using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class DivisionValueModifier : ValueModifier
    {
        public DivisionValueModifier(string id, string description, BigDouble value) : base(id, description, value.ToString(), true, priority: 2000)
        {

        }

        public DivisionValueModifier(string id, string description, string expression) : base(id, description, expression, priority: 2000)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression(engine);
            return ((BigDouble)input).Divide(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble) expressionResult);
        }
    }
}
