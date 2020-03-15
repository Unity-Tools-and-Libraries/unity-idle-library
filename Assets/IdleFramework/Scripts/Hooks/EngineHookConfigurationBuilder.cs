using IdleFramework;
using System;
using UnityEngine;

public class EngineHookConfigurationBuilder: EngineHookDefinitionProperties
{
    private Func<object, object> hook;
    private string actor;
    private EngineHookAction action;
    private string subject;

    public EngineHookAction Action => action;

    public string Actor => actor;

    public string Subject => subject;

    public static EngineHookTriggerTypeSelector When()
    {
        return new EngineHookTriggerTypeSelector(new EngineHookConfigurationBuilder());
    }

    public static Func<T, T> Logs<T>(string message)
    {
        return (T arg) =>
        {
            Debug.Log(message);
            return arg;
        };
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

    public class EngineHookEntityActionSelector
    {
        private EngineHookConfigurationBuilder parent;

        public EngineHookEntityActionSelector(EngineHookConfigurationBuilder parent)
        {
            this.parent = parent;
        }
        /*
         * Execute the trigger when the any
         */
        public EngineHookExecutionBuilder WillProduceAnyEntity()
        {
            parent.action = EngineHookAction.WILL_PRODUCE;
            parent.subject = "*";
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

    public EngineHookDefinition Build()
    {
        return new EngineHookDefinition(new EngineHookSelector(this.actor, this.action, this.subject), hook);
    }
}