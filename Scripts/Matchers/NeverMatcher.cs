namespace IdleFramework
{
    public class Never : StateMatcher
    {
        public bool Matches(IdleFramework.IdleEngine toCheck)
        {
            return false;
        }
    }
}