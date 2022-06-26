using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;

public static class ActionPhaseChangedEvent
{
    public const string EventName = "action_phase_changed";
    public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(string), "The new action phase."),
            Tuple.Create(typeof(string), "The previous action phase."),
        };
}
