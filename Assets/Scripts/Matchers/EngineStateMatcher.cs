namespace IdleFramework
{

    public abstract class EntityStateMatcher : StateMatcher
    {
        private readonly string entityKey;

        protected EntityStateMatcher(string entityKey)
        {
            this.entityKey = entityKey.ToLower();
        }

        public string EntityKey => entityKey;

        public abstract bool Matches(IdleEngine engine);
    }
}