using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class HookManager
    {
        private readonly IList<Action<IdleEngine>> startHooks;
        private readonly Dictionary<EngineHookEvent, Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>>> entityHooks = new Dictionary<EngineHookEvent, Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>>>();
        private readonly IList<Action<IdleEngine, float>> updateHooks;
        private readonly IList<Action<IdleEngine, float>> beforeUpdateHooks;
        private readonly Dictionary<string, IList<Action<IdleEngine, object>>> eventHooks;
        private readonly IdleEngine engine;
        public HookManager(HooksContainer hooks, IdleEngine engine)
        {
            this.engine = engine;
            startHooks = hooks.EngineStartHooks;
            entityHooks[EngineHookEvent.WILL_PRODUCE] = new Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>>();
            foreach (var entityHook in hooks.EntityProductionHooks)
            {
                setupEntityProductionHook(entityHook);
            }
            updateHooks = hooks.UpdateHooks;
            eventHooks = hooks.EventHooks;
            beforeUpdateHooks = hooks.BeforeUpdateHooks;
        }

        private void setupEntityProductionHook(EntityProductionHook entityHook)
        {
            Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>> productionHooks = entityHooks[EngineHookEvent.WILL_PRODUCE];
            Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>> hooksForActor;
            if (!productionHooks.TryGetValue(entityHook.Actor, out hooksForActor))
            {
                hooksForActor = new Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>();
                productionHooks[entityHook.Actor] = hooksForActor;
            }
            ISet<EngineHookDefinition<Entity, BigDouble>> hooksForSubject;
            if (!hooksForActor.TryGetValue(entityHook.Subject, out hooksForSubject))
            {
                hooksForSubject = new HashSet<EngineHookDefinition<Entity, BigDouble>>();
                hooksForActor[entityHook.Subject] = hooksForSubject;
            }
            productionHooks.Add(entityHook.Actor, hooksForActor);
        }

        public HookManager(IList<Action<IdleEngine>> startHooks, IList<EntityProductionHook> entityHooks)
        {
            this.startHooks = startHooks;
            foreach(var hook in entityHooks)
            {
                switch(hook.Action)
                {
                    case EngineHookEvent.WILL_PRODUCE:
                        Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>> hooksForAction;
                        if(!this.entityHooks.TryGetValue(hook.Selector.Action, out hooksForAction))
                        {
                            hooksForAction = new Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>>();
                            this.entityHooks.Add(hook.Selector.Action, hooksForAction);
                        }
                        Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>> hooksForEntity;
                        if(!hooksForAction.TryGetValue(hook.Selector.Actor, out hooksForEntity))
                        {
                            hooksForEntity = new Dictionary<string, ISet<EngineHookDefinition<Entity, BigDouble>>>();
                            hooksForAction.Add(hook.Selector.Actor, hooksForEntity);
                        }
                        ISet<EngineHookDefinition<Entity, BigDouble>> hookSet = null;
                        if(!hooksForEntity.TryGetValue(hook.Selector.Subject, out hookSet))
                        {
                            hookSet = new HashSet<EngineHookDefinition<Entity, BigDouble>>();
                            hooksForEntity.Add(hook.Selector.Subject, hookSet);
                        }
                        hookSet.Add(hook);
                        break;
                    default:
                        throw new InvalidOperationException(String.Format("Action type {0} not supported for entity hooks", hook.Action));
                }
            }
        }

        public  void ExecuteEventHook(string eventName, object arg)
        {
            IList<Action<IdleEngine, object>> hooksForNamedEvent;
            if(eventHooks.TryGetValue(eventName, out hooksForNamedEvent))
            {
                foreach(var hook in hooksForNamedEvent)
                {
                    hook.Invoke(engine, arg);
                }
            }
        }

        public void ExecuteEngineStartHooks()
        {
            foreach(var hook in startHooks)
            {
                hook.Invoke(engine);
            }
        }

        public BigDouble ExecuteEntityProductionHooks(Entity entity)
        {
            return entity.QuantityChangePerSecond;
        }

        public void ExecuteUpdateHooks(float deltaTime)
        {
            foreach(var hook in updateHooks)
            {
                hook.Invoke(engine, deltaTime);
            }
        }

        public void ExecuteBeforeUpdateHooks(float deltaTime)
        {
            foreach (var hook in updateHooks)
            {
                hook.Invoke(engine, deltaTime);
            }
        }
    }
}