using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class MultiplicativeValueModifier : ValueModifier
    {
        public MultiplicativeValueModifier(string id, string description, BigDouble value, ContextGenerator contextGenerator = null, int priority = ValueModifier.DefaultPriorities.MULTIPLICATION ) : base(id, description, value.ToString(), true, priority: priority, contextGenerator: contextGenerator)
        {

        }

        public MultiplicativeValueModifier(string id, string description, string expression, ContextGenerator contextGenerator = null, int priority = ValueModifier.DefaultPriorities.MULTIPLICATION) : base(id, description, expression, priority: priority, contextGenerator: contextGenerator)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression<object>(engine, container);
            return ((BigDouble)input).Multiply(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble)expressionResult);
        }
    }
}