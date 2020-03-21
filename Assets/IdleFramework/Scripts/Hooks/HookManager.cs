using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class HookManager
    {
        private readonly List<EngineStartHook> startHooks = new List<EngineStartHook>();
        private readonly Dictionary<EngineHookAction, Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>>> entityHooks = new Dictionary<EngineHookAction, Dictionary<string, Dictionary<string, ISet<EngineHookDefinition<GameEntity, BigDouble>>>>>();
        private readonly IdleEngine engine;
        public HookManager(HooksContainer hooks, IdleEngine engine) : this(hooks.EngineStartHooks, hooks.EntityProductionHooks)
        {
            this.engine = engine;
        }
        public HookManager(IList<EngineStartHook> startHooks, IList<EntityProductionHook> entityHooks)
        {
            foreach (var hook in startHooks)
            {
                switch (hook.Action)
                {
                    case EngineHookAction.ENGINE_START:
                        this.startHooks.Add(hook);
                        break;
                    default:
                        throw new InvalidOperationException("Action types other than ENGINE_START not supported in start hooks");
                }
            }
            foreach(var hook in entityHooks)
            {
                switch(hook.Action)
                {
                    case EngineHookAction.WILL_PRODUCE:
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

        public void ExecuteEngineStartHooks()
        {
            foreach(var hook in startHooks)
            {
                var returnValue = hook.Execute(null);
                if(returnValue != null)
                {
                    throw new InvalidOperationException("Engine Start hooks cannot return a value.");
                }
            }
        }

        public BigDouble ExecuteEntityProductionHooks(GameEntity entity)
        {
            return entity.QuantityChangePerSecond.GetAsNumber(engine);
        }
    }
}