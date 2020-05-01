using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class HookManager
    {
        private readonly List<Action<IdleEngine>> startHooks;
        private readonly List<Action<IdleEngine, float>> updateHooks;
        private readonly List<Action<IdleEngine, float>> beforeUpdateHooks;
        private readonly Dictionary<string, List<Action<Entity, IdleEngine>>> beforeBuyHooks;
        private readonly Dictionary<string, List<Action<IdleEngine, object>>> eventHooks;
        private readonly IdleEngine engine;
        public HookManager(HooksConfigurationContainer hooks, IdleEngine engine)
        {
            this.engine = engine;
            startHooks = hooks.EngineStartHooks;
            updateHooks = hooks.UpdateHooks;
            beforeUpdateHooks = hooks.BeforeUpdateHooks;
            beforeBuyHooks = hooks.BeforeBuyHooks;
        }

        public  void ExecuteEventHook(string eventName, object arg)
        {
            List<Action<IdleEngine, object>> hooksForNamedEvent;
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

        internal void ExecuteBeforeBuyHooks(Entity gameEntity)
        {
            Logger.GetLogger().Trace(string.Format("Executing beforeBuy hooks for {0}", gameEntity.EntityKey));
            List<Action<Entity, IdleEngine>> entitySpecificHooks;
            if(beforeBuyHooks.TryGetValue(gameEntity.EntityKey, out entitySpecificHooks))
            {
                foreach(var specificHook in entitySpecificHooks)
                {
                    specificHook.Invoke(gameEntity, engine);
                }
            }
            List<Action<Entity, IdleEngine>> generalHooks;
            if (beforeBuyHooks.TryGetValue("*", out generalHooks))
            {
                foreach (var generalHook in generalHooks)
                {
                    generalHook.Invoke(gameEntity, engine);
                }
            }
            Logger.GetLogger().Trace(string.Format("Executing beforeBuy hooks for {0}", gameEntity.EntityKey));
        }
    }
}