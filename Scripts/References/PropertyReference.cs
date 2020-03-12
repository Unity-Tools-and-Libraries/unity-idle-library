using BreakInfinity;

namespace IdleFramework
{
    public interface PropertyReference
    {
        BigDouble Get(IdleEngine engine);
    }

    public static class PropertyReferenceExtensions
    {
        public static PropertyReference Times(this PropertyReference left, PropertyReference right) => new ProductOf(left, right);
    }
}