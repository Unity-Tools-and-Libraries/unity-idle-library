using System;

namespace IdleFramework
{
    public class TutorialConfiguration
    {
        private readonly string tutorialKey;
        private readonly StateMatcher initiatedStateMatcher;
        private readonly Action<object> onInitiatedAction;
        private readonly StateMatcher completedStateMatcher;
        private readonly Action<object> onCompletedAction;

        public TutorialConfiguration(string tutorialKey, StateMatcher initiatedStateMatcher, Action<object> onInitiatedAction, StateMatcher completedStateMatcher, Action<object> onCompletedAction)
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

        public Action<object> OnInitiatedAction => onInitiatedAction;

        public Action<object> OnCompletedAction => onCompletedAction;
    }
}