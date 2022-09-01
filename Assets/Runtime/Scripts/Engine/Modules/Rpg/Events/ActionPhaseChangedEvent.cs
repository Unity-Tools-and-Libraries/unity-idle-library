using io.github.thisisnozaku.scripting.context;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    /*
     * Event emitted when the action phase changes.
     */
    public class ActionPhaseChangedEvent : IScriptingContext
    {
        public const string EventName = "action_phase_changed";

        public Dictionary<string, object> GetContextVariables()
        {
            throw new NotImplementedException();
        }
    }
}