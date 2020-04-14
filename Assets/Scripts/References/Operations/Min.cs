using System.Collections.Generic;
using BreakInfinity;
namespace IdleFramework
{
    public class Min : NumberContainer
    {
        private NumberContainer[] children;

        private Min(params NumberContainer[] children)
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

        public static Min Of(params NumberContainer[] children)
        {
            return new Min(children);
        }
    }
}