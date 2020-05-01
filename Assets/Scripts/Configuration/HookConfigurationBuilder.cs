using Assets.IdleFramework.Scripts.Hooks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class HookConfigurationBuilder
    {
        private readonly List<Action<IdleEngine>> engineStartHooks = new List<Action<IdleEngine>>();
        private readonly List<Action<IdleEngine, float>> updateHooks = new List<Action<IdleEngine, float>>();
        private readonly List<Action<IdleEngine, float>> beforeUpdateHooks = new List<Action<IdleEngine, float>>();
        private readonly Dictionary<string, List<Action<IdleEngine, float>>> customEventHooks = new Dictionary<string, List<Action<IdleEngine, float>>>();
        private readonly Dictionary<string, List<Action<Entity, IdleEngine>>> beforeBuyHooks = new Dictionary<string, List<Action<Entity, IdleEngine>>>();

        public IReadOnlyList<Action<IdleEngine>> EngineStartHooks => engineStartHooks;

        public void AddStartHook(Action<IdleEngine> hook)
        {
            engineStartHooks.Add(hook);
        }

        public HooksConfigurationContainer Build()
        {
            return new HooksConfigurationContainer(engineStartHooks, updateHooks, beforeUpdateHooks, beforeBuyHooks);
        }

        public void AddUpdateHook(Action<IdleEngine, float> hook)
        {
            updateHooks.Add(hook);
        }

        public void AddBeforeUpdateHook(Action<IdleEngine, float> hook)
        {
            updateHooks.Add(hook);
        }

        internal void AddCustomEventHook(string customEventName, Action<IdleEngine, float> hook)
        {
            List<Action<IdleEngine, float>> hooks;
            if(!customEventHooks.TryGetValue(customEventName, out hooks)) {
                hooks = new List<Action<IdleEngine, float>>();
                customEventHooks.Add(customEventName, hooks);
            }
            hooks.Add(hook);
        }

        internal void AddBeforeEntityBuyHook(string entityName, Action<Entity, IdleEngine> hook)
        {
            List<Action<Entity, IdleEngine>> hooks;
            if(!beforeBuyHooks.TryGetValue(entityName, out hooks))
            {
                hooks = new List<Action<Entity, IdleEngine>>();
                beforeBuyHooks.Add(entityName, hooks);
            }
            hooks.Add(hook);
        }
    }
}