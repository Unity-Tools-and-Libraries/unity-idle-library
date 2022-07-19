using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class EngineEventTests : TestsRequiringEngine
    {

        [Test]
        public void EmitOnEngineInvokesListeners()
        {
            engine.Watch("event", "test", "triggered = true");
            engine.Emit("event", engine);
            Assert.IsTrue(engine.Scripting.EvaluateString("return triggered").Boolean);
        }

        [Test]
        public void InvokeListenersForEngineReadyOnStart()
        {
            engine.Watch(EngineReadyEvent.EventName, "test", "triggered = true");
            engine.Start();
            Assert.IsTrue(engine.Scripting.EvaluateString("return triggered").Boolean);
        }

        [Test]
        public void SubscribingToEngineReadyInvokedImmediatelyWhenEngineIsReady()
        {
            engine.Start();
            engine.Watch(EngineReadyEvent.EventName, "test", "triggered = true");
            Assert.IsTrue(engine.Scripting.EvaluateString("return triggered").Boolean);
        }

        [Test]
        public void CanStopWatching()
        {
            engine.Watch("event", "test", "triggered = true");
            engine.StopWatching("event", "test");
            engine.Emit("test", engine);
            Assert.IsFalse(engine.Scripting.EvaluateString("return triggered").Boolean);
        }

        [Test]
        public void CanEmitUsingDictionaryContext()
        {
            engine.Watch("event", "test", "triggered = true");
            engine.StopWatching("event", "test");
            engine.Emit("test", new Dictionary<string, object>());
            Assert.IsFalse(engine.Scripting.EvaluateString("return triggered").Boolean);
        }
    }
}