using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.scripting.context;
using UnityEngine;

public class MaxLevelReachedEvent : IScriptingContext
{
    public const string EventName = "MaxLevelReached";
    private Dictionary<string, object> context;

    public MaxLevelReachedEvent(Upgrade upgrade, BigDouble level)
    {
        context = new Dictionary<string, object>()
        {
            { "level", level },
            { "upgrade", upgrade }
        };
    }

    public Dictionary<string, object> GetContextVariables()
    {
        return context;
    }
}
