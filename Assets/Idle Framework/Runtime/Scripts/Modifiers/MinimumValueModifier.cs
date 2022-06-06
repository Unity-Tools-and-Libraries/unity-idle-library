using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class MinimumValueModifier : ValueModifier
    {
        public MinimumValueModifier(string id, string source, string expression, bool staticValue = false, ContextGenerator contextGenerator = null, int priority = 0) : base(id, source, expression, staticValue, priority: priority, contextGenerator: contextGenerator)
        {
        }

        public MinimumValueModifier(string id, string source, BigDouble value, ContextGenerator contextGenerator = null, int priority = 0) : base(id, source, value.ToString(), true, priority: priority, contextGenerator: contextGenerator)
        {
        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return BigDouble.Max(EvaluateCalculationExpression<BigDouble>(engine, container), (BigDouble)input);
        }
    }
}