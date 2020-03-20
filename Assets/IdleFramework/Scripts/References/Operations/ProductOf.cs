using BreakInfinity;
namespace IdleFramework
{
    public class Product
        : PropertyReference
    {
        private PropertyReference left;
        private PropertyReference right;

        private Product(PropertyReference left, PropertyReference right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            return left.GetAsNumber(engine) * right.GetAsNumber(engine);
        }

        public static Product Of(PropertyReference left, PropertyReference right)
        {
            return new Product(left, right);
        }
    }
}