using BreakInfinity;
using System;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    /*
     * A ContainerModifier which only modifies the calculated value of the target container.
     */
    public abstract class ValueModifier : ContainerModifier
    {
        protected object cachedValue;
        private string expression;
        protected bool IsStaticValue;
        protected ValueModifier(string id, string source, string expression, bool staticValue = false, int priority = 0) : base(id, source, priority)
        {
            this.expression = expression;
            this.IsStaticValue = staticValue;
        }

        public override abstract object Apply(IdleEngine engine, ValueContainer container, object input);

        protected object EvaluateCalculationExpression(IdleEngine engine)
        {
            if(IsStaticValue && cachedValue != null)
            {
                return cachedValue;
            }
            var evaluatedExpression = engine.EvaluateExpression(expression);
            if(IsStaticValue && cachedValue == null)
            {
                cachedValue = evaluatedExpression;
            }
            return ValueContainer.NormalizeValue(evaluatedExpression);
        }

        public static class DefaultPriorities
        {
            public const int MULTIPLICATION = 2000;
            public const int ADDITION = 1000;
        }
    }
}