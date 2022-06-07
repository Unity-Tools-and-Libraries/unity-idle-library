using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.ValueContainer.Context;

namespace io.github.thisisnozaku.idle.framework.Modifiers.Values
{
    /*
     * A ContainerModifier which only modifies the calculated value of the target container.
     */
    public abstract class ValueModifier : ContainerModifier
    {
        protected object cachedValue;
        private string expression;
        protected ValueModifier(string id, string source, string expression, bool staticValue = false, ContextGenerator contextGenerator = null, int priority = 0) : base(id, source, contextGenerator: contextGenerator, priority: priority)
        {
            this.expression = expression;
            this.IsStatic= staticValue;
        }

        public override abstract object Apply(IdleEngine engine, ValueContainer container, object input);

        protected T EvaluateCalculationExpression<T>(IdleEngine engine, ValueContainer container)
        {
            if(IsStatic && cachedValue != null)
            {
                return (T)cachedValue;
            }
            var evaluatedExpression = engine.EvaluateExpression(expression, this.GenerateContext(engine, container));
            if(IsStatic && cachedValue == null)
            {
                cachedValue = evaluatedExpression;
            }
            return (T)ValueContainer.NormalizeValue(evaluatedExpression);
        }

        public static class DefaultPriorities
        {
            public const int MULTIPLICATION = 2000;
            public const int ADDITION = 1000;
        }
    }
}