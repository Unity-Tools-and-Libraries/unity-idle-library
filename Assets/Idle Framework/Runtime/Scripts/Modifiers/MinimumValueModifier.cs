using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class MinimumValueModifier : ValueModifier
    {
        public MinimumValueModifier(string id, string source, string expression, bool staticValue = false, int priority = 0) : base(id, source, expression, staticValue, priority)
        {
        }

        public MinimumValueModifier(string id, string source, BigDouble value, int priority = 0) : base(id, source, value.ToString(), true, priority)
        {
        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return BigDouble.Max((BigDouble)EvaluateCalculationExpression(engine), (BigDouble)input);
        }
    }
}