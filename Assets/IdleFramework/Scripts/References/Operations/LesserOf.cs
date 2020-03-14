using BreakInfinity;

namespace IdleFramework
{
    public class LesserOf<T> : PropertyReference
    {
        private PropertyReference[] references;

        public LesserOf(params PropertyReference[] references) => this.references = references;

        public BigDouble Get(IdleEngine engine)
        {
            if(references.Length == 0)
            {
                return 0;
            }
            BigDouble smallest = references[0].Get(engine);
            foreach (var reference in references)
            {
                var nextValue = reference.Get(engine);
                if (smallest == null || smallest.CompareTo(nextValue) > 0)
                {
                    smallest = reference.Get(engine) ;
                }
            }
            return smallest;
        }
    }

}