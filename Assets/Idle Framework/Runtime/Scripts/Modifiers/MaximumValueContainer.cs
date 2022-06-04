using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Modifiers
{

    public class MaximumValueContainer : ValueModifier
    {
        public MaximumValueContainer(string id, string source, string expression, bool staticValue = false, int priority = 0) : base(id, source, expression, staticValue, priority)
        {
        }

        public MaximumValueContainer(string id, string source, BigDouble value, int priority = 0) : base(id, source, value.ToString(), true, priority)
        {
        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return BigDouble.Min((BigDouble)EvaluateCalculationExpression(engine), (BigDouble)input);
        }
    }
}