using BreakInfinity;
namespace IdleFramework
{
    public class MinOf : PropertyReference
    {
        private PropertyReference[] children;

        public MinOf(params PropertyReference[] children)
        {
            this.children = children;
        }

        public BigDouble Get(IdleEngine engine)
        {
            if(children.Length == 0 )
            {
                return 0;
            }
            var smallest = children[0].Get(engine);
            foreach(var reference in children)
            {
                smallest = BigDouble.Min(smallest, reference.Get(engine));
            }
            return smallest;
        }
    }
}