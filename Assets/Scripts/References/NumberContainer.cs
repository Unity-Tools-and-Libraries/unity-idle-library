using BreakInfinity;
using System;

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

        public static NumberContainer Plus(this NumberContainer left, NumberContainer right)
        {
            return Sum.Of(left, right);
        }

        public static NumberContainer Minus(this NumberContainer left, NumberContainer right)
        {
            return Difference.Between(left, right);
        }

        public static NumberContainer Minus(this NumberContainer left, BigDouble right)
        {
            return Difference.Between(left, Literal.Of(right));
        }

        public static NumberContainer DividedBy(this NumberContainer left, NumberContainer right)
        {
            return Ratio.Of(left, right);
        }

        public static NumberContainer LogarithmOf(this NumberContainer left, NumberContainer @base)
        {
            return Logarithm.Of(@base, left);
        }

        public static NumberContainer ToPower(this NumberContainer left, NumberContainer power)
        {
            return Exponent.Of(left, power);
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

        public static StringContainer AsFormattedString(this NumberContainer left, string formatSpecifier)
        {
            return new FormattedNumber(left, formatSpecifier);
        }
    }
}