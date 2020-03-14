namespace IdleFramework
{

    public abstract class EntityStateMatcher : StateMatcher
    {
        private readonly string entityKey;

        public string EntityKey => entityKey;

        public abstract bool Matches(IdleEngine engine);
    }
}