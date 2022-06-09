using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{

    public class MaximumValueContainer : ValueModifier
    {
        public MaximumValueContainer(string id, string source, string expression, string[] dependencies = null, ContextGenerator contextGenerator = null, int priority = 0) : base(id, source, expression, dependencies, contextGenerator: contextGenerator, priority: priority)
        {
        }

        public MaximumValueContainer(string id, string source, BigDouble value, ContextGenerator contextGenerator = null, int priority = 0) : base(id, source, value.ToString(), null, contextGenerator: contextGenerator, priority: priority)
        {
        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return BigDouble.Min(EvaluateCalculationExpression<BigDouble>(engine, container), (BigDouble)input);
        }
    }
}