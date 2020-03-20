using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{
    public class Literal : PropertyReference
    {
        private static readonly Dictionary<BigDouble, Literal> cache = new Dictionary<BigDouble, Literal>();
        private BigDouble value;

        private Literal(BigDouble value)
        {
            this.value = value;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return value;
        }

        public static implicit operator Literal(BigDouble value) => new Literal(value);
        public static implicit operator Literal(int value) => new Literal(new BigDouble(value));
        public static implicit operator Literal(float value) => new Literal(new BigDouble(value));

        public static Literal Of(BigDouble value)
        {
            Literal literal;
            if(!cache.TryGetValue(value, out literal))
            {
                literal = new Literal(value);
                cache.Add(value, literal);
            }
            return literal;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Literal ({0})", value);
        }
    }
}