using IdleFramework.Configuration;

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
            engine.Log(string.Format("Calling update for tutorial '{0}'", configuration.TutorialKey), Logger.Level.TRACE);
            switch (status)
            {
                case Status.AVAILABLE:
                    if (configuration.InitiatedStateMatcher.Matches(engine))
                    {
                        if (configuration.OnInitiatedAction != null)
                        {
                            engine.Log(string.Format("Invoking initiation hook for tutorial '{0}'", configuration.TutorialKey), Logger.Level.TRACE);
                            configuration.OnInitiatedAction(engine);
                        }
                        status = Status.ACTIVE;
                        engine.Log(string.Format("Switching tutorial '{0}' status to ACTIVE.", configuration.TutorialKey), Logger.Level.TRACE);
                    }
                    break;
                case Status.ACTIVE:
                    if (configuration.CompletedStateMatcher.Matches(engine))
                    {
                        if (configuration.OnCompletedAction != null)
                        {
                            engine.Log(string.Format("Invoking completion hook for tutorial '{0}'", configuration.TutorialKey), Logger.Level.TRACE);
                            configuration.OnCompletedAction(engine);
                        }
                        status = Status.COMPLETE;
                        engine.Log(string.Format("Switching tutorial '{0}' status to COMPLETE.", configuration.TutorialKey), Logger.Level.TRACE);
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