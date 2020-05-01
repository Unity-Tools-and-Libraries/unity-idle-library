using System;

namespace IdleFramework.Configuration
{
    public class TutorialConfigurationBuilder : Builder<TutorialConfiguration>
    {
        private string tutorialKey;
        private StateMatcher initiatedMatcher = Always.Instance;
        private Action<IdleEngine> onInitiatedAction;
        private StateMatcher completedMatcher = Always.Instance;
        private Action<IdleEngine> onCompletedAction;

        public TutorialConfigurationBuilder(string tutorialKey)
        {
            this.tutorialKey = tutorialKey;
        }

        public TutorialConfigurationBuilder InitiatesWhen(StateMatcher initiatesWhen)
        {
            initiatedMatcher = initiatesWhen;
            return this;
        }

        public TutorialConfigurationBuilder ExecutesHookOnInitiation(Action<IdleEngine> hook)
        {
            onInitiatedAction = hook;
            return this;
        }

        public TutorialConfigurationBuilder CompletesWhen(StateMatcher matcher)
        {
            this.completedMatcher = matcher;
            return this;
        }

        public TutorialConfigurationBuilder ExecutesHookOnCompletion(Action<IdleEngine> hook)
        {
            onCompletedAction = hook;
            return this;
        }

        public TutorialConfiguration Build()
        {
            return new TutorialConfiguration(tutorialKey, initiatedMatcher, onInitiatedAction, completedMatcher, onCompletedAction);
        }
    }
}