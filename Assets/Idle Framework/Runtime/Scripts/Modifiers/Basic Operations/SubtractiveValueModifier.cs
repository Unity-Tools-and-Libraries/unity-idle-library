using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class SubtractiveValueModifier : ValueModifier
    {
        public SubtractiveValueModifier(string id, string description, BigDouble value, ContextGenerator contextGenerator = null) : base(id, description, value.ToString(), null, contextGenerator: contextGenerator, priority: ValueModifier.DefaultPriorities.ADDITION)
        {

        }

        public SubtractiveValueModifier(string id, string description, string expression, string[] dependencies = null, ContextGenerator contextGenerator = null) : base(id, description, expression, dependencies, contextGenerator: contextGenerator, priority: ValueModifier.DefaultPriorities.ADDITION)
        {

        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression<object>(engine, container);
            return ((BigDouble)input).Subtract(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble)expressionResult);
        }
    }
}