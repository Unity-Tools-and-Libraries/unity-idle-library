using io.github.thisisnozaku.scripting.context;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class EncounterStartedEvent : IScriptingContext
    {
        public const string EventName = "encounterStarted";

        public Dictionary<string, object> GetContextVariables()
        {
            return null;
        }
    }

    public class EncounterEntedEvent : IScriptingContext
    {
        public const string EventName = "encounterEnded";

        public Dictionary<string, object> GetContextVariables()
        {
            return null;
        }
    }
}