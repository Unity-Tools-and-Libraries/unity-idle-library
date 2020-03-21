namespace IdleFramework
{
    /*
     * Selector for determining when a hook should be executed.
     */
    public class EngineHookSelector
    {
        private readonly string actor;
        private readonly EngineHookAction action;
        private readonly string subject;

        public string Actor { get => actor; }
        public EngineHookAction Action { get => action; }
        public string Subject { get => subject; }

        public EngineHookSelector(EngineHookAction action, string actor, string subject)
        {
            this.actor = actor;
            this.action = action;
            this.subject = subject;
        }

        public EngineHookSelector(EngineHookAction action)
        {
            this.action = action;
        }
    }
}