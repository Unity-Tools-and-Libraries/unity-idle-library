using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.scripting.context;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using UnityEngine;

public class ProducerInstance : IEventSource
{
    public double ProducerId;
    public bool IsUnlocked;
    public bool IsEnabled;
    public BigDouble Quantity;
    public BigDouble OutputMultiplier = BigDouble.One;
    public string UnitOutputScript;
    private IdleEngine Engine;
    [JsonProperty]
    private EventListeners listeners;

    [JsonConstructor]
    private ProducerInstance()
    {

    }

    public ProducerInstance(IdleEngine engine, double Id) : base()
    {
        this.ProducerId = Id;
        UnitOutputScript = engine.GetProducers()[Id].UnitOutputScript;
        this.Engine = engine;
        this.listeners = new EventListeners(this.Engine, false);
    }

    [OnDeserialized]
    public void OnDeserialization(StreamingContext ctx)
    {
        this.Engine = (IdleEngine)ctx.Context;
        this.listeners = new EventListeners(this.Engine, false);
        UnitOutputScript = Engine.GetProducers()[ProducerId].UnitOutputScript;
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

    public void Watch(string eventName, string watcherIdentifier, string handler)
    {
        ((IEventSource)listeners).Watch(eventName, watcherIdentifier, handler);
    }

    public void Watch(string eventName, string subscriber, DynValue handler)
    {
        ((IEventSource)listeners).Watch(eventName, subscriber, handler);
    }

    public void StopWatching(string eventName, string subscriptionIdentifier)
    {
        ((IEventSource)listeners).StopWatching(eventName, subscriptionIdentifier);
    }

    [JsonIgnore]
    public BigDouble TotalOutput => Engine.Scripting.EvaluateStringAsScript(UnitOutputScript).ToObject<BigDouble>() * Quantity * OutputMultiplier;

    
}
