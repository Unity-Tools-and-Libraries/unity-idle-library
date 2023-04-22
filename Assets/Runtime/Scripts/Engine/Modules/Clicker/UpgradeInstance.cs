using System;
using System.Collections;
using System.Collections.Generic;
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

    public UpgradeInstance(double upgradeId)
    {
        UpgradeId = upgradeId;
    }

    public void Emit(string eventName, IScriptingContext contextToUse = null)
    {
        throw new NotImplementedException();
    }

    public void Emit(string eventName, IDictionary<string, object> contextToUse)
    {
        throw new NotImplementedException();
    }

    public void Emit(string eventName, Tuple<string, object> contextToUse)
    {
        throw new NotImplementedException();
    }

    public void StopWatching(string eventName, string subscriptionIdentifier)
    {
        throw new NotImplementedException();
    }

    public void Watch(string eventName, string watcherIdentifier, string handler)
    {
        throw new NotImplementedException();
    }

    public void Watch(string eventName, string subscriber, DynValue handler)
    {
        throw new NotImplementedException();
    }
}
