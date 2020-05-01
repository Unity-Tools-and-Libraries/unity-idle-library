using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class HooksConfigurationContainer
    {
        private readonly List<Action<IdleEngine>> engineStartHooks;
        private readonly List<Action<IdleEngine, float>> updateHooks;
        private readonly List<Action<IdleEngine, float>> beforeUpdateHooks;
        private readonly Dictionary<string, List<Action<Entity, IdleEngine>>> beforeBuyHooks;

        public HooksConfigurationContainer(List<Action<IdleEngine>> engineStartHooks, 
            List<Action<IdleEngine, float>> updateHooks,
            List<Action<IdleEngine, float>> beforeUpdateHooks,
            Dictionary<string, List<Action<Entity, IdleEngine>>> beforeBuyHooks)
        {
            this.engineStartHooks = engineStartHooks;
            this.updateHooks = updateHooks;
            this.beforeUpdateHooks = beforeUpdateHooks;
            this.beforeBuyHooks = beforeBuyHooks;
        }

        public List<Action<IdleEngine>> EngineStartHooks => engineStartHooks;

        public List<Action<IdleEngine, float>> UpdateHooks => updateHooks;

        public List<Action<IdleEngine, float>> BeforeUpdateHooks => beforeUpdateHooks;

        public Dictionary<string, List<Action<Entity, IdleEngine>>> BeforeBuyHooks => beforeBuyHooks;
    }
}