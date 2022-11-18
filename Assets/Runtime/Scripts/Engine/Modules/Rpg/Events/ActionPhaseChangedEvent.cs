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
        public const string NewActionPhase = "newActionPhase";
        public const string PreviousActionPhase = "previousActionPhase";

        private Dictionary<string, object> vars;
        public ActionPhaseChangedEvent(string newActionPhase, string previousActionPhase)
        {
            vars = new Dictionary<string, object>()
            {
                { NewActionPhase, newActionPhase },
                { PreviousActionPhase, previousActionPhase }
            };
        }

        public Dictionary<string, object> GetContextVariables()
        {
            return vars;
        }
    }
}