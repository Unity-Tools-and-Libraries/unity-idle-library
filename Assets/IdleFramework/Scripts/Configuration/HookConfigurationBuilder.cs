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
        private readonly List<EntityProductionHook> entityProductionHooks = new List<EntityProductionHook>();
        private readonly List<Action<IdleEngine, float>> updateHooks = new List<Action<IdleEngine, float>>();
        private readonly Dictionary<string, IList<Action<IdleEngine, object>>> eventHooks = new Dictionary<string, IList<Action<IdleEngine, object>>>();

        public IReadOnlyList<Action<IdleEngine>> EngineStartHooks => engineStartHooks;

        public IReadOnlyList<EntityProductionHook> EntityProductionHooks => entityProductionHooks;

        public void AddStartHook(Action<IdleEngine> hook)
        {
            engineStartHooks.Add(hook);
        }

        public void AddEntityProductionHook(EntityProductionHook hook)
        {
            entityProductionHooks.Add(hook);
        }

        internal HooksContainer Build()
        {
            return new HooksContainer(engineStartHooks, entityProductionHooks, updateHooks, eventHooks);
        }

        internal void AddUpdateHook(Action<IdleEngine, float> hook)
        {
            updateHooks.Add(hook);
        }

        internal void AddEventHook(string eventName, Action<IdleEngine, object> hook)
        {
            IList<Action<IdleEngine, object>> hooks;
            if(!eventHooks.TryGetValue(eventName, out hooks))
            {
                hooks = new List<Action<IdleEngine, object>>();
                eventHooks.Add(eventName, hooks);
            }
            hooks.Add(hook);
        }
    }
}