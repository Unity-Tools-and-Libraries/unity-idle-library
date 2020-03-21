using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class HooksContainer
    {
        private readonly List<EngineStartHook> engineStartHooks;
        private readonly List<EntityProductionHook> entityProductionHooks;

        public HooksContainer(List<EngineStartHook> engineStartHooks, 
            List<EntityProductionHook> entityProductionHooks)
        {
            this.engineStartHooks = engineStartHooks;
            this.entityProductionHooks = entityProductionHooks;
        }

        public IList<EngineStartHook> EngineStartHooks => engineStartHooks.AsReadOnly();

        public IList<EntityProductionHook> EntityProductionHooks => entityProductionHooks.AsReadOnly();
    }
}