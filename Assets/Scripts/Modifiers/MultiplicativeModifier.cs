using BreakInfinity;
using System;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class MultiplicativeModifier : Modifier
    {
        public MultiplicativeModifier(string id, string description, BigDouble value) : base(id, description, value)
        {
        }

        public override object Apply(object input)
        {
            if(!(input is BigDouble))
            {
                throw new InvalidOperationException();
            }
            return ((BigDouble)input).Multiply(ValueAsNumber());
        }
    }
}