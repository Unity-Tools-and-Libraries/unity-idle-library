using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    public class MultiplicativeValueModifier : ValueModifier
    {
        public MultiplicativeValueModifier(string id, string description, BigDouble value) : base(id, description, value.ToString(), true, priority: ValueModifier.DefaultPriorities.MULTIPLICATION)
        {

        }

        public MultiplicativeValueModifier(string id, string description, string expression) : base(id, description, expression, priority: ValueModifier.DefaultPriorities.MULTIPLICATION)
        {

        }
        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            if (!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            var expressionResult = EvaluateCalculationExpression(engine);
            return ((BigDouble)input).Multiply(expressionResult is ValueContainer ? (expressionResult as ValueContainer).ValueAsNumber() : (BigDouble)expressionResult);
        }
    }
}