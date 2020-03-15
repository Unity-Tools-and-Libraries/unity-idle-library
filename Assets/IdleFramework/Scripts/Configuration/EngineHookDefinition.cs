using System;

namespace IdleFramework
{
    /*
     * Configuration for an engine hook.
     */
    public class EngineHookDefinition : EngineHookDefinitionProperties
    {
        private readonly EngineHookSelector selector;
        private readonly Func<object, object> function;

        public EngineHookDefinition(EngineHookSelector selector, Func<object, object> hook)
        {
            this.selector = selector;
            this.function = hook;
        }

        public EngineHookSelector Selector => selector;
        public Func<object, object> Function => function;

        public EngineHookAction Action => selector.Action;

        public string Actor => selector.Actor;

        public string Subject => selector.Subject;

        public object Execute(object arg) => function(arg);
    }
}