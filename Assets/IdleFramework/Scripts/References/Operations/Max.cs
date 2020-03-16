using BreakInfinity;
namespace IdleFramework
{
    public class Max : PropertyReference
    {
        private PropertyReference[] children;

        private Max(params PropertyReference[] children)
        {
            this.children = children;
        }

        public BigDouble Get(IdleEngine engine)
        {
            if (children.Length == 0)
            {
                return 0;
            }
            var largest = children[0].Get(engine);
            foreach (var reference in children)
            {
                largest = BigDouble.Max(largest, reference.Get(engine));
            }
            return largest;
        }

        public static Max Of(params PropertyReference[] references)
        {
            return new Max(references);
        }
    }
}