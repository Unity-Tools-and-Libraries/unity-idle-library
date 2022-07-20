using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    /*
     * Event emitted when the action phase changes.
     */
    public class ActionPhaseChangedEvent : ScriptingContext
    {
        public const string EventName = "action_phase_changed";

        public Dictionary<string, object> GetScriptingProperties()
        {
            throw new NotImplementedException();
        }
    }
}