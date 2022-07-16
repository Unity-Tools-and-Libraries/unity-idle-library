using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Scripting
{
    public class ScriptingTests : TestsRequiringEngine
    {
        [SetUp]
        public void setup()
        {
            base.InitializeEngine();
        }

        [Test]
        public void CanAssignValue()
        {

            engine.Scripting.Evaluate("foo = 1");
            Assert.AreEqual(new BigDouble(1), engine.GetProperty<BigDouble>("foo"));

            engine.GlobalProperties["foo"] = new Dictionary<string, object>();
            engine.Scripting.Evaluate("foo.bar = 1");
            Assert.AreEqual(1, engine.GetProperty<double>("foo.bar"));
        }

        [Test]
        public void CanGetValue()
        {
            engine.GlobalProperties["foo"] = new Dictionary<string, object>();
            engine.GetProperty<IDictionary<string, object>>("foo")["bar"] = new BigDouble(1);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return foo.bar").ToObject<BigDouble>());
        }

        [Test]
        public void CanSubscribeToEventsEmittedByEntity()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.Evaluate("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.Evaluate("foo.emit('event')");

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanSubscribeToEventsReceivedAtTheEngine()
        {
            engine.Scripting.Evaluate("engine.watch('event', 'test', 'triggered = true')");
            engine.Scripting.Evaluate("engine.emit('event', engine)");
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanUnsubscribeFromPath()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.Evaluate("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.Evaluate("foo.stopWatching('event', 'test')");
            engine.Scripting.Evaluate("foo.emit('event')");
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
        }

        [Test]
        public void CanBroadcastEvent()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.Evaluate("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.Evaluate("foo.emit('event')");
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanCalculateValue()
        {
            engine.Start();
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.Evaluate("foo.calculateChild('Bar', 'if value != nil then return value + 1 else return 1 end')");
            engine.Update(0);
            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return foo.bar").ToObject<BigDouble>());
        }

        [Test]
        public void CalculatorCalledEachUpdateCycle()
        {
            engine.Start();
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1, 0);
            engine.Scripting.Evaluate("foo.calculateChild('Bar', 'if value == nil then return 1 else return value + 1 end')");
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(0);
                Assert.AreEqual(new BigDouble(i + 1), engine.Scripting.Evaluate("return foo.bar").ToObject<BigDouble>());
            }
        }

        [Test]
        public void BuiltInMaxReturnsLargestOfTwoNumbers()
        {
            engine.GlobalProperties["foo"] = 1;
            engine.GlobalProperties["bar"] = 2;
            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return math.max(foo, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return math.max(1, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return math.max(foo, 2)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return math.max(1, 2)").ToObject<BigDouble>());
        }
        [Test]
        public void BuiltInMinReturnsSmallestOfTwoNumbers()
        {
            engine.GlobalProperties["foo"] = 1;
            engine.GlobalProperties["bar"] = 2;
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return math.min(foo, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return math.min(1, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return math.min(foo, 2)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return math.min(1, 2)").ToObject<BigDouble>());
        }

        [Test]
        public void BuiltInClampReturnsLargestOfValueAndFloor()
        {
            Assert.AreEqual(new BigDouble(15), engine.Scripting.Evaluate("return math.clamp(15, 10, 20)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(10), engine.Scripting.Evaluate("return math.clamp(5, 10, 20)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(20), engine.Scripting.Evaluate("return math.clamp(25, 10, 20)").ToObject<BigDouble>());
        }

        [Test]
        public void BuiltInPowReturnsPower()
        {
            Assert.AreEqual(new BigDouble(5).Pow(2), engine.Scripting.Evaluate("return math.pow(5, 2)").ToObject<BigDouble>());
        }

        [Test]
        public void StringToBigDoubleParsesTheString()
        {
            Assert.AreEqual(new BigDouble(10), engine.Scripting.Evaluate("return '10'").ToObject<BigDouble>());
            Assert.IsTrue(BigDouble.IsNaN(engine.Scripting.Evaluate("return 'chungus'").ToObject<BigDouble>()));
        }

        [Test]
        public void NilToBigDoubleReturns0()
        {
            Assert.AreEqual(BigDouble.Zero, engine.Scripting.Evaluate("return nil").ToObject<BigDouble>());
        }

        [Test]
        public void UserDataToBigDoubleThrows()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.Scripting.Evaluate("return value", new Dictionary<string, object>()
                {
                    { "value", new TestEntity(engine, 1) }
                }).ToObject<BigDouble>();
            });
        }

        [Test]
        public void ContextCanBeASingleKeyValuePair()
        {
            Assert.IsTrue(engine.Scripting.Evaluate("return value", new KeyValuePair<string, object>("value", true)).Boolean);
        }

        [Test]
        public void DoesntLikeNullScript()
        {
            Assert.Throws(typeof(ArgumentNullException), () => {
                engine.Scripting.Evaluate(null);
            });
        }
    }
}