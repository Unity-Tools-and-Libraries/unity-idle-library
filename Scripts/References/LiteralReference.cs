using BreakInfinity;

namespace IdleFramework
{
    public class LiteralReference : PropertyReference
    {
        private BigDouble value;

        public LiteralReference(BigDouble value)
        {
            this.value = value;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return value;
        }

        public static implicit operator LiteralReference(BigDouble value) => new LiteralReference(value);
        public static implicit operator LiteralReference(int value) => new LiteralReference(new BigDouble(value));
        public static implicit operator LiteralReference(float value) => new LiteralReference(new BigDouble(value));
    }
}