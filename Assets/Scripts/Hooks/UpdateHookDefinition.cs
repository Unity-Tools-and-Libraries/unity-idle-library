using IdleFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.IdleFramework.Scripts.Hooks
{
    public class UpdateHookDefinition
    {
        private readonly Action<IdleEngine, float> hook;
        public UpdateHookDefinition(Action<IdleEngine, float> hook)
        {
            this.hook = hook;
        }

        public Action<IdleEngine, float> Hook => hook;
    }
}