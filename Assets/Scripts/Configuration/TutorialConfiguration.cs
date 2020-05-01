using System;

namespace IdleFramework.Configuration
{
    public class TutorialConfiguration
    {
        private readonly string tutorialKey;
        private readonly StateMatcher initiatedStateMatcher;
        private readonly Action<IdleEngine> onInitiatedAction;
        private readonly StateMatcher completedStateMatcher;
        private readonly Action<IdleEngine> onCompletedAction;

        public TutorialConfiguration(string tutorialKey, StateMatcher initiatedStateMatcher, Action<IdleEngine> onInitiatedAction, StateMatcher completedStateMatcher, Action<IdleEngine> onCompletedAction)
        {
            this.tutorialKey = tutorialKey;
            this.initiatedStateMatcher = initiatedStateMatcher;
            this.onInitiatedAction = onInitiatedAction;
            this.completedStateMatcher = completedStateMatcher;
            this.onCompletedAction = onCompletedAction;
        }

        public string TutorialKey => tutorialKey;

        public StateMatcher InitiatedStateMatcher => initiatedStateMatcher;

        public StateMatcher CompletedStateMatcher => completedStateMatcher;

        public Action<IdleEngine> OnInitiatedAction => onInitiatedAction;

        public Action<IdleEngine> OnCompletedAction => onCompletedAction;
    }
}