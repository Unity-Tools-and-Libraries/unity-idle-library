using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class HookConfigurationBuilder
    {
        private readonly List<EngineStartHook> engineStartHooks = new List<EngineStartHook>();
        private readonly List<EntityProductionHook> entityProductionHooks = new List<EntityProductionHook>();

        public IReadOnlyList<EngineStartHook> EngineStartHooks => engineStartHooks;

        public IReadOnlyList<EntityProductionHook> EntityProductionHooks => entityProductionHooks;

        public void AddStartHook(EngineStartHook hook)
        {
            engineStartHooks.Add(hook);
        }

        public void AddEntityProductionHook(EntityProductionHook hook)
        {
            entityProductionHooks.Add(hook);
        }

        internal HooksContainer Build()
        {
            return new HooksContainer(engineStartHooks, entityProductionHooks);
        }
    }
}