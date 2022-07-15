using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events
{
    public class IsEnabledChangedEvent : ScriptingContext
    {
        public const string EventName = "IsEnabledChanged";
        private IEnableable enableable;

        public IsEnabledChangedEvent(IEnableable enableable)
        {
            this.enableable = enableable;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "enableable", enableable }
            };
        }
    }
}