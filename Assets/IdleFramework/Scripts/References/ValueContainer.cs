using BreakInfinity;

namespace IdleFramework
{
    public interface ValueContainer
    {
        bool GetAsBoolean(IdleEngine engine);
        BigDouble GetAsNumber(IdleEngine engine);
        string GetAsString(IdleEngine engine);
        PropertyContainer GetAsContainer(IdleEngine engine);
        object RawValue(IdleEngine engine);
    }

    public static class ValueContainerExtensions
    {
        public static ValueContainer Times(this ValueContainer left, ValueContainer right) => Product.Of(left, right);
        public static ValueContainer Minus(this ValueContainer left, ValueContainer right) => Difference.Between(left, right);
        public static ValueContainer Plus(this ValueContainer left, ValueContainer right) => Sum.Of(left, right);
        public static ValueContainer DividedBy(this ValueContainer left, ValueContainer right) => Ratio.Of(left, right);
        public static ValueContainer AsContainer(this BigDouble value) => Literal.Of(value);
        public static ValueContainer AsContainer(this bool value) => Literal.Of(value);
        public static ValueContainer AsContainer(this string value) => Literal.Of(value);
        public static ValueContainer ClampedBetween(this ValueContainer value, ValueContainer min, ValueContainer max) => Clamped.Of(value, min, max);
        }

}