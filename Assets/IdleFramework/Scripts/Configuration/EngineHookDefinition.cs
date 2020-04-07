using System;

namespace IdleFramework
{
    /*
     * Configuration for an engine hook.
     */
    public abstract class EngineHookDefinition<I, O> : EngineHookDefinitionProperties<I, O>
    {
        private readonly EngineHookSelector selector;
        private readonly Func<I, O> function;

        public EngineHookDefinition(EngineHookSelector selector, Func<I, O> hook)
        {
            this.selector = selector;
            this.function = hook;
        }

        public EngineHookSelector Selector => selector;
        public Func<I, O> Function => function;

        public EngineHookEvent Action => selector.Action;

        public string Actor => selector.Actor;

        public string Subject => selector.Subject;

        public O Execute(I arg) => function(arg);

        public abstract class Builder
        {
            public abstract EngineHookTriggerConfigurationBuilder When();
        }

        public abstract class EngineHookTriggerConfigurationBuilder
        {
            
        }
    }
}