using BreakInfinity;
namespace IdleFramework
{
    public class Min : PropertyReference
    {
        private ValueContainer[] children;

        private Min(params ValueContainer[] children)
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

        public static Min Of(params ValueContainer[] children)
        {
            return new Min(children);
        }

        public bool GetAsBoolean(IdleEngine toCheck)
        {
            return BigDouble.Zero.Equals(GetAsNumber(toCheck));
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