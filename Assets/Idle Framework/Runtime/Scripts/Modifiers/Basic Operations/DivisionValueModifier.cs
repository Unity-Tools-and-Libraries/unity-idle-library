using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class DivisionValueModifier : ValueModifier
    {
        public DivisionValueModifier(string id, string description, BigDouble value, ContextGenerator contextGenerator = null) : base(id, description, value.ToString(), true, contextGenerator: contextGenerator, priority: 2000)
        {

        }

        public DivisionValueModifier(string id, string description, string expression, ContextGenerator contextGenerator = null) : base(id, description, expression, contextGenerator: contextGenerator, priority: 2000)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression<object>(engine, container);
            return ((BigDouble)input).Divide(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble) expressionResult);
        }
    }
}
