using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineEventTests : TestsRequiringEngine
{
    
    [Test]
    public void EmitOnEngineInvokesListeners()
    {
        engine.Watch("event", "test", "triggered = true");
        engine.Emit("event", engine);
        Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
    }
    
    [Test]
    public void InvokeListenersForEngineReadyOnStart()
    {
        engine.Watch(EngineReadyEvent.EventName, "test", "triggered = true");
        engine.Start();
        Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
    }

    [Test]
    public void SubscribingToEngineReadyInvokedImmediatelyWhenEngineIsReady()
    {
        engine.Start();
        engine.Watch(EngineReadyEvent.EventName, "test", "triggered = true");
        Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
    }

    [Test]
    public void CanStopWatching()
    {
        engine.Watch("event", "test", "triggered = true");
        engine.StopWatching("event", "test");
        engine.Emit("test", engine);
        Assert.IsFalse(engine.Scripting.Evaluate("return triggered").Boolean);
    }
}
