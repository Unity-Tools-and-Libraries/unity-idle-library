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

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            if (children.Length == 0)
            {
                return 0;
            }
            var largest = children[0].GetAsNumber(engine);
            foreach (var reference in children)
            {
                largest = BigDouble.Max(largest, reference.GetAsNumber(engine));
            }
            return largest;
        }

        public static Max Of(params PropertyReference[] references)
        {
            return new Max(references);
        }
    }
}