namespace IdleFramework
{
    public class TutorialConfiguration
    {
        private readonly StateMatcher triggerMatcher;

        public TutorialConfiguration(StateMatcher triggerMatcher)
        {
            this.triggerMatcher = triggerMatcher;
        }

        public StateMatcher TriggerMatcher => triggerMatcher;
    }
}