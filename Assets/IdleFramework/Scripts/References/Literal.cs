using BreakInfinity;

namespace IdleFramework
{
    public class Literal : PropertyReference
    {
        private BigDouble value;

        private Literal(BigDouble value)
        {
            this.value = value;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return value;
        }

        public static implicit operator Literal(BigDouble value) => new Literal(value);
        public static implicit operator Literal(int value) => new Literal(new BigDouble(value));
        public static implicit operator Literal(float value) => new Literal(new BigDouble(value));

        public static Literal Of(BigDouble value)
        {
            return new Literal(value);
        }
    }
}