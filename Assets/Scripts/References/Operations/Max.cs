using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{
    public class Max : NumberContainer
    {
        private NumberContainer[] children;

        private Max(params NumberContainer[] children)
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

        public static Max Of(params NumberContainer[] references)
        {
            return new Max(references);
        }
    }
}