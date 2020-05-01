using IdleFramework.Configuration;
using System;

namespace IdleFramework.Hooks
{
    public class EngineStartHook : EngineHookDefinition<object, object>
    {
        public EngineStartHook(Func<object, object> hook) : base(new EngineHookSelector(EngineHookEvent.ENGINE_START), hook)
        {
            
        }
    }
}