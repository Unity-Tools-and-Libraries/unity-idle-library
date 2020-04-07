using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class HooksContainer
    {
        private readonly List<Action<IdleEngine>> engineStartHooks;
        private readonly List<EntityProductionHook> entityProductionHooks;
        private readonly List<Action<IdleEngine, float>> updateHooks;
        private readonly Dictionary<string, IList<Action<IdleEngine, object>>> eventHooks;

        public HooksContainer(List<Action<IdleEngine>> engineStartHooks, 
            List<EntityProductionHook> entityProductionHooks,
            List<Action<IdleEngine, float>> updateHooks,
            Dictionary<string, IList<Action<IdleEngine, object>>> eventHooks)
        {
            this.engineStartHooks = engineStartHooks;
            this.entityProductionHooks = entityProductionHooks;
            this.updateHooks = updateHooks;
            this.eventHooks = eventHooks;
        }

        public IList<Action<IdleEngine>> EngineStartHooks => engineStartHooks.AsReadOnly();

        public IList<EntityProductionHook> EntityProductionHooks => entityProductionHooks.AsReadOnly();

        public List<Action<IdleEngine, float>> UpdateHooks => updateHooks;

        public Dictionary<string, IList<Action<IdleEngine, object>>> EventHooks => eventHooks;
    }
}