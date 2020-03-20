using BreakInfinity;

namespace IdleFramework
{
    public interface PropertyReference
    {
        BigDouble GetAsNumber(IdleEngine engine);
    }

    public static class PropertyReferenceExtensions
    {
        public static PropertyReference Times(this PropertyReference left, PropertyReference right) => Product.Of(left, right);

        public static PropertyReference Minus(this PropertyReference left, PropertyReference right) => Difference.Of(left, right);
    }
}