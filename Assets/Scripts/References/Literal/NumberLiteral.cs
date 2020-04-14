using BreakInfinity;

namespace IdleFramework
{
    public class NumberLiteral : NumberContainer
    {
        private readonly BigDouble value;

        public NumberLiteral(BigDouble value)
        {
            this.value = value;
        }
        public override string ToString()
        {
            return string.Format("Literal {0}", value);
        }

        public BigDouble Get(IdleEngine engine)
        {
            return value;
        }

        public override bool Equals(object obj)
        {
            return obj is NumberLiteral literal &&
                   value.Equals(literal.value);
        }
    }
}