using BreakInfinity;

namespace IdleFramework
{
    public interface NumberContainer : ValueContainer
    {
        BigDouble Get(IdleEngine engine);
    }

    public static class NumberExtensions
    {
        public static NumberContainer Times(this NumberContainer left, NumberContainer right)
        {
            return Product.Of(left, right);
        }

        public static NumberContainer AsNumber(this ValueContainer left)
        {
            if (left is NumberContainer)
            {
                return (NumberContainer)left;
            }
            return Literal.Of(0);
        }

        public static BooleanContainer AsBoolean(this ValueContainer left)
        {
            if (left is BooleanContainer)
            {
                return (BooleanContainer)left;
            }
            return Literal.Of(false);
        }

        public static StringContainer AsString(this ValueContainer left)
        {
            if (left is StringContainer)
            {
                return (StringContainer)left;
            }
            return Literal.Of("");
        }
    }
}