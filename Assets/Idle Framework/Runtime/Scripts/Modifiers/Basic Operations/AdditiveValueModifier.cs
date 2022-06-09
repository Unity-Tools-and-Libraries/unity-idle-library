using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class AdditiveValueModifier : ValueModifier
    {
        public AdditiveValueModifier(string id, string description, BigDouble value, ContextGenerator contextGenerator = null, int priority = ValueModifier.DefaultPriorities.ADDITION) : base(id, description, value.ToString(), null, contextGenerator: contextGenerator, priority: priority)
        {
            
        }

        public AdditiveValueModifier(string id, string description, string expression, string[] dependencies = null, ContextGenerator contextGenerator = null, int priority = ValueModifier.DefaultPriorities.ADDITION) : base(id, description, expression, dependencies, contextGenerator: contextGenerator, priority: priority)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression<object>(engine, container);
            return ((BigDouble)input).Add(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble)expressionResult);
        }
    }
}