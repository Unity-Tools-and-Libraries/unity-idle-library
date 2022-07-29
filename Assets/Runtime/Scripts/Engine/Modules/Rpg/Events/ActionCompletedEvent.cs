using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Events;
using UnityEngine;

public class ActionCompletedEvent : ScriptingContext
{
    public const string EventName = "ActionCompleted";

    public readonly string Action;

    public ActionCompletedEvent(string action)
    {
        this.Action = action;
    }

    public Dictionary<string, object> GetScriptingProperties()
    {
        return new Dictionary<string, object>();
    }
}
