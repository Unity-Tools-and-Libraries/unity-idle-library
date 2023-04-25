using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.scripting.context;
using MoonSharp.Interpreter;
using UnityEngine;

public class UpgradeInstance : IEventSource
{
    public double UpgradeId;
    public bool IsEnabled;
    public bool IsUnlocked;
    public BigDouble Quantity;
    private IdleEngine Engine;

    private EventListeners listeners;

    public UpgradeInstance(IdleEngine engine, double upgradeId)
    {
        this.Engine = engine;
        UpgradeId = upgradeId;
        listeners = new EventListeners(engine, false);
    }

    public void Emit(string eventName, IScriptingContext contextToUse = null)
    {
        ((IEventSource)listeners).Emit(eventName, contextToUse);
    }

    public void Emit(string eventName, IDictionary<string, object> contextToUse)
    {
        ((IEventSource)listeners).Emit(eventName, contextToUse);
    }

    public void Emit(string eventName, Tuple<string, object> contextToUse)
    {
        ((IEventSource)listeners).Emit(eventName, contextToUse);
    }

    [OnDeserialized]
    public void OnDeserialization(StreamingContext ctx)
    {
        this.Engine = (IdleEngine)ctx.Context;
    }

    public void StopWatching(string eventName, string subscriptionIdentifier)
    {
        ((IEventSource)listeners).StopWatching(eventName, subscriptionIdentifier);
    }

    public void Watch(string eventName, string watcherIdentifier, string handler)
    {
        ((IEventSource)listeners).Watch(eventName, watcherIdentifier, handler);
    }

    public void Watch(string eventName, string subscriber, DynValue handler)
    {
        ((IEventSource)listeners).Watch(eventName, subscriber, handler);
    }
}
