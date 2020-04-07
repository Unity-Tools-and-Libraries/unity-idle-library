using BreakInfinity;
namespace IdleFramework
{
    public class Max : PropertyReference
    {
        private ValueContainer[] children;

        private Max(params ValueContainer[] children)
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

        public static Max Of(params ValueContainer[] references)
        {
            return new Max(references);
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            throw new System.NotImplementedException();
        }

        public string GetAsString(IdleEngine engine)
        {
            return GetAsNumber(engine).ToString();
        }

        public object RawValue(IdleEngine engine)
        {
            return GetAsNumber(engine);
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }
    }
}