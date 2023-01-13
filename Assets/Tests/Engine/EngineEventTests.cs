using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class EngineEventTests : TestsRequiringEngine
    {

        [Test]
        public void EmitOnEngineInvokesListeners()
        {
            engine.Watch("event", "test", "globals.triggered = true");
            engine.Emit("event", engine);
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CanWatchWithACallback()
        {
            engine.Watch("event", "test", DynValue.FromObject(null, (Action)(() =>
            {
                engine.GlobalProperties["triggered"] = true;
            })));
            engine.Emit("event", engine);
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CanWatchWithACallbackForEngineReadyInvokesImmediatelyWhenEngineReady()
        {
            engine.Start();
            engine.Watch(EngineReadyEvent.EventName, "test", DynValue.FromObject(null, (Action<IDictionary<string, object>>)(ie =>
            {
                Assert.NotNull(ie);
                engine.GlobalProperties["triggered"] = true;
            })));
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CanWatchWithACallbackForEngineReadyDoesNotInvokeImmediatelyWhenEngineNotReady()
        {
            engine.Watch(EngineReadyEvent.EventName, "test", DynValue.FromObject(null, (Action<IdleEngine>)(ie =>
            {
                Assert.NotNull(ie);
                engine.GlobalProperties["triggered"] = true;
            })));
            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void WatchWithNonFunctionDynValueThrows()
        {
            Assert.Throws(typeof(ArgumentException), () =>
            {
                engine.Watch("event", "test", DynValue.FromObject(null, true));
            });
        }

        [Test]
        public void InvokeListenersForEngineReadyOnStart()
        {
            engine.Watch(EngineReadyEvent.EventName, "test", "globals.triggered = true");
            engine.Start();
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void SubscribingToEngineReadyInvokedImmediatelyWhenEngineIsReady()
        {
            engine.Start();
            engine.Watch(EngineReadyEvent.EventName, "test", "globals.triggered = true");
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CanStopWatching()
        {
            engine.Watch("event", "test", "globals.triggered = true");
            engine.StopWatching("event", "test");
            engine.Emit("test", engine);
            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CanStopWatchingWithACallback()
        {
            engine.Watch("event", "test", DynValue.FromObject(null, (Action)(() =>
            {
                engine.GlobalProperties["triggered"] = true;
            })));
            engine.StopWatching("event", "test");
            engine.Emit("test", engine);
            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return triggered").Boolean);
        }

        [Test]
        public void CanEmitUsingDictionaryContext()
        {
            engine.Watch("event", "test", "triggered = true");
            engine.StopWatching("event", "test");
            engine.Emit("test", new Dictionary<string, object>());
            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return triggered").Boolean);
        }

        [Test]
        public void CanEmitUsingTupleContext()
        {
            engine.Watch("event", "test", "triggered = true");
            engine.StopWatching("event", "test");
            engine.Emit("test", Tuple.Create<string, object>("foo", "bar"));
            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return triggered").Boolean);
        }
    }
}