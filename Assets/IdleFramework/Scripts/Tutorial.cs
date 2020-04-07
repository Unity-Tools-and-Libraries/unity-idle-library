using System;

namespace IdleFramework
{
    /*
     * State of a player tutorial.
     */
    public class Tutorial : Updates
    {
        private readonly TutorialConfiguration configuration;
        private Status status;

        public Tutorial(TutorialConfiguration configuration)
        {
            status = Status.AVAILABLE;
            this.configuration = configuration;
        }

        public TutorialConfiguration Configuration => configuration;

        public Status Progress => status;

        public void Update(IdleEngine engine, float deltaTime)
        {
            switch (status)
            {
                case Status.AVAILABLE:
                    if (configuration.InitiatedStateMatcher.Matches(engine))
                    {
                        status = Status.ACTIVE;
                        configuration.OnInitiatedAction(null);
                        goto case Status.ACTIVE;
                    }
                    break;
                case Status.ACTIVE:
                    if (configuration.CompletedStateMatcher.Matches(engine))
                    {
                        configuration.OnCompletedAction(null);
                        status = Status.COMPLETE;
                    }
                    break;
            }
        }

        public enum Status
        {
            /*
             * The tutorial has not started, but can start.
             */
            AVAILABLE,
            /*
             * The tutorial has started and is not yet complete.
             */
            ACTIVE,
            /*
             * The tutorial has been completed.
             */
            COMPLETE
        }

        public void Complete()
        {
            if(status == Status.ACTIVE)
            {
                status = Status.COMPLETE;
            }
        }

        public void Start()
        {
            if(status == Status.AVAILABLE)
            {
                status = Status.ACTIVE;
            }
        }
    }
}