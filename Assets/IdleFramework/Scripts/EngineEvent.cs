namespace IdleFramework
{
    public class EngineEvent
    {
        private EngineHookActor actor;
        private EngineHookAction action;
        private string subject;
        private object payload;

        public EngineEvent(EngineHookActor actor, EngineHookAction action, string subject, object payload)
        {
            this.actor = actor;
            this.action = action;
            this.subject = subject;
            this.payload = payload;
        }

        public EngineHookActor Actor { get => actor; set => actor = value; }
        public EngineHookAction Action { get => action; set => action = value; }
        public string Subject { get => subject; set => subject = value; }
        public object Payload { get => payload; set => payload = value; }
    }
}