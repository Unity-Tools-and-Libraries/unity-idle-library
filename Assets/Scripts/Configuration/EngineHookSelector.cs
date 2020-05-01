using IdleFramework.Hooks;

namespace IdleFramework.Configuration
{
    /*
     * Selector for determining when a hook should be executed.
     */
    public class EngineHookSelector
    {
        private readonly string actor;
        private readonly EngineHookEvent action;
        private readonly string subject;

        public string Actor { get => actor; }
        public EngineHookEvent Action { get => action; }
        public string Subject { get => subject; }

        public EngineHookSelector(EngineHookEvent action, string actor, string subject)
        {
            this.actor = actor;
            this.action = action;
            this.subject = subject;
        }

        public EngineHookSelector(EngineHookEvent action)
        {
            this.action = action;
        }
    }
}