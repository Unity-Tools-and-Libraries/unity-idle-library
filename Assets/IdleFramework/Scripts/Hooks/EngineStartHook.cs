using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EngineStartHook : EngineHookDefinition<object, object>
    {
        public EngineStartHook(Func<object, object> hook) : base(new EngineHookSelector(EngineHookAction.ENGINE_START), hook)
        {
            
        }
    }
}