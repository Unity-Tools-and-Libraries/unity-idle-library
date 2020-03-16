namespace IdleFramework
{
    public class Never : StateMatcher
    {
        private static readonly Never instance = new Never();
        private Never() { }
        public static Never Instance => instance;

        public bool Matches(IdleFramework.IdleEngine toCheck)
        {
            return false;
        }
    }
}