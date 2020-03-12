using BreakInfinity;
namespace IdleFramework
{
    public class ProductOf : PropertyReference
    {
        private PropertyReference left;
        private PropertyReference right;

        public ProductOf(PropertyReference left, PropertyReference right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return left.Get(engine) * right.Get(engine);
        }
    }
}