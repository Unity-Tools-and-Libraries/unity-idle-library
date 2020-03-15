using IdleFramework;
using System;

public class EngineHookConfigurationBuilder
{
    private Func<object, object> hook;
    private string actor;
    private EngineHookAction action;
    private string subject;
    public static EngineHookTriggerTypeSelector When()
    {
        return new EngineHookTriggerTypeSelector(new EngineHookConfigurationBuilder());
    }

    public class EngineHookTriggerTypeSelector
    {
        private EngineHookConfigurationBuilder parent;
        
        public EngineHookTriggerTypeSelector(EngineHookConfigurationBuilder parent)
        {
            this.parent = parent;
        }

        public EngineHookEntityActionSelector AnyEntity()
        {
            parent.actor = "*";
            return new EngineHookEntityActionSelector(parent);
        }
    }

    public class EngineHookEntityActionSelector : EngineHookConfigurationBuilder
    {
        private EngineHookConfigurationBuilder parent;

        public EngineHookEntityActionSelector(EngineHookConfigurationBuilder parent)
        {
            this.parent = parent;
        }
        /*
         * Execute the trigger when the any
         */
        public EngineHookExecutionBuilder ProducesAnyEntity()
        {
            return new EngineHookExecutionBuilder(parent);
        }
    }

    public class EngineHookExecutionBuilder
    {
        private EngineHookConfigurationBuilder parent;

        public EngineHookExecutionBuilder(EngineHookConfigurationBuilder parent)
        {
            this.parent = parent;
        }

        public EngineHookConfigurationBuilder ThenExecute(Func<object, object> hook)
        {
            parent.hook = hook;
            return parent;
        }
    }

    internal EngineHookDefinition Build()
    {
        return new EngineHookDefinition(new EngineHookSelector(this.actor, this.action, this.subject), hook);
    }
}