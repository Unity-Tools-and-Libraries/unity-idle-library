using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Configuration
{
    public class EntityHooksBuilder
    {
        private readonly IList<Action<Entity, IdleEngine>> beforeUpdateHooks = new List<Action<Entity, IdleEngine>>();

        public IList<Action<Entity, IdleEngine>> BeforeUpdateHooks => beforeUpdateHooks;

        internal void AddBeforeUpdateHook(Action<Entity, IdleEngine> hook)
        {
            beforeUpdateHooks.Add(hook);
        }
    }
}