using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.scripting.context;
using MoonSharp.Interpreter;
using UnityEngine;

public class ProducerInstance : IEventSource
{
    public double ProducerId;
    public bool IsUnlocked;
    public bool IsEnabled;
    public BigDouble Quantity;
    public BigDouble OutputMultiplier;

    public ProducerInstance(double Id)
    {
        this.ProducerId = Id;
    }

    public BigDouble CalculateOutput(IdleEngine engine)
    {
        var producer = engine.GetProducers()[ProducerId];
        
        return engine.Scripting.EvaluateStringAsScript(producer.UnitOutputScript).ToObject<BigDouble>() * Quantity;
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
