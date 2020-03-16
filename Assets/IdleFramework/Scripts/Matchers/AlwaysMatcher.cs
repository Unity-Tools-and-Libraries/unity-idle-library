namespace IdleFramework
{
    public class Always : StateMatcher
    {
        public static readonly Always instance = new Always();
        private Always()
        {

        }

        public bool Matches(IdleEngine toCheck)
        {
            return true;
        }

        public static Always Instance => instance;

    }
}