using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class EncounterStartedEvent : ScriptingContext
    {
        public const string EventName = "encounterStarted";

        public Dictionary<string, object> GetScriptingProperties()
        {
            return null;
        }
    }

    public class EncounterEntedEvent : ScriptingContext
    {
        public const string EventName = "encounterEnded";

        public Dictionary<string, object> GetScriptingProperties()
        {
            return null;
        }
    }
}