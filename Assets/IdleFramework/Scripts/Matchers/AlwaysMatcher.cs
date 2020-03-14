namespace IdleFramework
{
    public class Always : StateMatcher
    {
        public override bool Equals(object obj)
        {
            if (typeof(Always).Equals(obj.GetType()))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Matches(IdleEngine toCheck)
        {
            return true;
        }


    }
}