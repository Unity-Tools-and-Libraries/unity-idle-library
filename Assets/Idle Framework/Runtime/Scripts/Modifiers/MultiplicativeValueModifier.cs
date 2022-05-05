using BreakInfinity;
using System;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class MultiplicativeValueModifier : ValueModifier
    {
        private BigDouble value;
        public MultiplicativeValueModifier(string id, string description, BigDouble value) : base(id, description, 200)
        {
            this.value = value;
        }

        public override object Apply(object input)
        {
            if(!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            return ((BigDouble)input).Multiply(value);
        }
    }
}