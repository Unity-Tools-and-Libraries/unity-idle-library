using System;

namespace IdleFramework
{
    public class TutorialConfigurationBuilder
    {
        private string tutorialKey;
        private string message;
        private StateMatcher availableMatcher;
        private Action<object> onAvailableAction;
        private StateMatcher initiatedMatcher;
        private Action<object> onInitiatedAction;
        private StateMatcher completedMatcher;
        private Action<object> onCompletedAction;

        public TutorialConfigurationBuilder(string tutorialKey)
        {
            this.tutorialKey = tutorialKey;
        }

        public TutorialInitiationSelector InitiatesWhen(StateMatcher initiatesWhen)
        {
            initiatedMatcher = initiatesWhen;
            return new TutorialInitiationSelector(this);
        }

        public class TutorialInitiationSelector
        {
            private TutorialConfigurationBuilder parent;

            public TutorialInitiationSelector(TutorialConfigurationBuilder parent)
            {
                this.parent = parent;
            }

            public TutorialInitiationSelector WhichExecutes(Action<object> onInitiatedAction)
            {
                parent.onInitiatedAction = onInitiatedAction;
                return this;
            }

            public TutorialCompletionActionSelector AndCompletesWhen(StateMatcher completesWhen)
            {
                parent.completedMatcher = completesWhen;
                return new TutorialCompletionActionSelector(parent);
            }
        }

        public class TutorialCompletionActionSelector
        {
            private TutorialConfigurationBuilder parent;

            public TutorialCompletionActionSelector(TutorialConfigurationBuilder parent)
            {
                this.parent = parent;
            }

            public TutorialConfigurationTerminal WhichExecutes(Action<object> onCompletion)
            {
                parent.onCompletedAction = onCompletion;
                return new TutorialConfigurationTerminal(parent);
            }
        }

        public class TutorialConfigurationTerminal
        {
            private TutorialConfigurationBuilder parent;

            public TutorialConfigurationTerminal(TutorialConfigurationBuilder parent)
            {
                this.parent = parent;
            }

            public TutorialConfiguration Build()
            {
                return new TutorialConfiguration(parent.tutorialKey,
                    parent.initiatedMatcher, parent.onInitiatedAction,
                    parent.completedMatcher, parent.onCompletedAction);
            }
        }
    }
}