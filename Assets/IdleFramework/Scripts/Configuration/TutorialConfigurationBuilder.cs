using System;

namespace IdleFramework
{
    public class TutorialConfigurationBuilder
    {
        private string message;
        private StateMatcher triggerMatcher;
        private Action action;

        public TutorialActionConfigurationBuilder WhenGameStarts()
        {
            triggerMatcher = Always.Instance;
            return new TutorialActionConfigurationBuilder(this);
        }

        public class TutorialActionConfigurationBuilder
        {
            private readonly TutorialConfigurationBuilder parent;

            public TutorialActionConfigurationBuilder(TutorialConfigurationBuilder parent)
            {
                this.parent = parent;
            }

            public TerminalTutorialConfigurationBuilderStage ThenDisplay(string messageToDisplay)
            {
                parent.message = messageToDisplay;
                return new TerminalTutorialConfigurationBuilderStage(parent);
            }

            public TerminalTutorialConfigurationBuilderStage ThenExecute(Action action)
            {
                this.parent.action = action;
                return new TerminalTutorialConfigurationBuilderStage(parent);
            }
        }

        public class TerminalTutorialConfigurationBuilderStage
        {
            private TutorialConfigurationBuilder parent;
            public TerminalTutorialConfigurationBuilderStage(TutorialConfigurationBuilder parent)
            {
                this.parent = parent;
            }

            public TutorialConfiguration Build()
            {
                return new TutorialConfiguration(parent.triggerMatcher);
            }
        }
    }
}