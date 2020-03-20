using BreakInfinity;
namespace IdleFramework
{
    public class Min : PropertyReference
    {
        private PropertyReference[] children;

        private Min(params PropertyReference[] children)
        {
            this.children = children;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            if(children.Length == 0 )
            {
                return 0;
            }
            var smallest = children[0].GetAsNumber(engine);
            foreach(var reference in children)
            {
                smallest = BigDouble.Min(smallest, reference.GetAsNumber(engine));
            }
            return smallest;
        }

        public static Min Of(params PropertyReference[] children)
        {
            return new Min(children);
        }
    }
}