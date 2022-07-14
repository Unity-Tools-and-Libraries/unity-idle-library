using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterStartedEvent : ScriptingContext
{
    public const string EventName = "encounter_started";

    public Dictionary<string, object> GetScriptingProperties()
    {
        return new Dictionary<string, object>();
    }
}
