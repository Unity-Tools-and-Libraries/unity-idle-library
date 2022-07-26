using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Events;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class EncounterEndedEvent: ScriptingContext
    {
        public const string EventName = "encounter_ended";

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>();
        }
    }
}