using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.scripting.context;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events
{
    public class IsUnlockedChangeEvent : IScriptingContext
    {
        public const string EventName = "IsUnlockedChanged";
        private IUnlockable unlockable;

        public IsUnlockedChangeEvent(IUnlockable unlockable)
        {
            this.unlockable = unlockable;
        }

        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>()
            {
                { "unlockable", unlockable }
            };
        }
    }
}