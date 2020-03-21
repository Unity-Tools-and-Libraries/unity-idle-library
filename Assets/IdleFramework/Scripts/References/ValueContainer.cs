using BreakInfinity;

namespace IdleFramework
{
    public interface ValueContainer
    {
        bool GetAsBoolean(IdleEngine engine);
        BigDouble GetAsNumber(IdleEngine engine);
        string GetAsString(IdleEngine engine);
        object RawValue(IdleEngine engine);
    }

    public static class ValueContainerExtensions
    {
        public static ValueContainer Times(this ValueContainer left, ValueContainer right) => Product.Of(left, right);
        public static ValueContainer Minus(this ValueContainer left, ValueContainer right) => Difference.Of(left, right);
        public static ValueContainer AsContainer(this BigDouble value) => Literal.Of(value);
        public static ValueContainer AsContainer(this bool value) => Literal.Of(value);
        public static ValueContainer AsContainer(this string value) => Literal.Of(value);

    }
}