using System.Collections.Generic;
using io.github.thisisnozaku.scripting.context;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class EncounterEndedEvent: IScriptingContext
    {
        public const string EventName = "encounter_ended";

        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}