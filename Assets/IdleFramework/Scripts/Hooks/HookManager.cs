using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class HookManager
    {
        private readonly IList<Action<IdleEngine>> startHooks;
        private readonly Dictionary<EngineHookEvent, Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>>> entityHooks = new Dictionary<EngineHookEvent, Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>>>();
        private readonly IList<Action<IdleEngine, float>> updateHooks;
        private readonly Dictionary<string, IList<Action<IdleEngine, object>>> eventHooks;
        private readonly IdleEngine engine;
        public HookManager(HooksContainer hooks, IdleEngine engine)
        {
            this.engine = engine;
            startHooks = hooks.EngineStartHooks;
            entityHooks[EngineHookEvent.WILL_PRODUCE] = new Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>>();
            foreach (var entityHook in hooks.EntityProductionHooks)
            {
                setupEntityProductionHook(entityHook);
            }
            updateHooks = hooks.UpdateHooks;
            eventHooks = hooks.EventHooks;
        }

        private void setupEntityProductionHook(EntityProductionHook entityHook)
        {
            Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>> productionHooks = entityHooks[EngineHookEvent.WILL_PRODUCE];
            Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>> hooksForActor;
            if (!productionHooks.TryGetValue(entityHook.Actor, out hooksForActor))
            {
                hooksForActor = new Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>();
                productionHooks[entityHook.Actor] = hooksForActor;
            }
            ISet<EngineHookDefinition<GameEntity, BigDouble>> hooksForSubject;
            if (!hooksForActor.TryGetValue(entityHook.Subject, out hooksForSubject))
            {
                hooksForSubject = new HashSet<EngineHookDefinition<GameEntity, BigDouble>>();
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
                        Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>> hooksForAction;
                        if(!this.entityHooks.TryGetValue(hook.Selector.Action, out hooksForAction))
                        {
                            hooksForAction = new Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>>();
                            this.entityHooks.Add(hook.Selector.Action, hooksForAction);
                        }
                        Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>> hooksForEntity;
                        if(!hooksForAction.TryGetValue(hook.Selector.Actor, out hooksForEntity))
                        {
                            hooksForEntity = new Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>();
                            hooksForAction.Add(hook.Selector.Actor, hooksForEntity);
                        }
                        ISet<EngineHookDefinition<GameEntity, BigDouble>> hookSet = null;
                        if(!hooksForEntity.TryGetValue(hook.Selector.Subject, out hookSet))
                        {
                            hookSet = new HashSet<EngineHookDefinition<GameEntity, BigDouble>>();
                            hooksForEntity.Add(hook.Selector.Subject, hookSet);
                        }
                        hookSet.Add(hook);
                        break;
                    default:
                        throw new InvalidOperationException(String.Format("Action type {0} not supported for entity hooks", hook.Action));
                }
            }
        }

        internal void ExecuteEventHook(string eventName, object arg)
        {
            
        }

        public void ExecuteEngineStartHooks()
        {
            foreach(var hook in startHooks)
            {
                hook.Invoke(engine);
            }
        }

        public BigDouble ExecuteEntityProductionHooks(GameEntity entity)
        {
            return entity.QuantityChangePerSecond.GetAsNumber(engine);
        }

        internal void ExecuteUpdateHooks(float deltaTime)
        {
            foreach(var hook in updateHooks)
            {
                hook.Invoke(engine, deltaTime);
            }
        }
    }
}