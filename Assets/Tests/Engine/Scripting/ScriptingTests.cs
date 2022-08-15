using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Scripting;
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

            engine.Scripting.EvaluateStringAsScript("foo = 1");
            Assert.AreEqual(new BigDouble(1), engine.GetProperty<BigDouble>("foo"));

            engine.GlobalProperties["foo"] = new Dictionary<string, object>();
            engine.Scripting.EvaluateStringAsScript("foo.bar = 1");
            Assert.AreEqual(1, engine.GetProperty<double>("foo.bar"));
        }

        [Test]
        public void CanGetValue()
        {
            engine.GlobalProperties["foo"] = new Dictionary<string, object>();
            engine.GetProperty<IDictionary<string, object>>("foo")["bar"] = new BigDouble(1);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return foo.bar").ToObject<BigDouble>());
        }

        [Test]
        public void CanSubscribeToEventsEmittedByEntity()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.EvaluateStringAsScript("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.EvaluateStringAsScript("foo.emit('event')");

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanSubscribeToEventsReceivedAtTheEngine()
        {
            engine.Scripting.EvaluateStringAsScript("engine.watch('event', 'test', 'triggered = true')");
            engine.Scripting.EvaluateStringAsScript("engine.emit('event', engine)");
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanUnsubscribeFromPath()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.EvaluateStringAsScript("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.EvaluateStringAsScript("foo.stopWatching('event', 'test')");
            engine.Scripting.EvaluateStringAsScript("foo.emit('event')");
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
        }

        [Test]
        public void CanBroadcastEvent()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.EvaluateStringAsScript("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.EvaluateStringAsScript("foo.emit('event')");
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanCalculateValue()
        {
            engine.Start();
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            engine.Scripting.EvaluateStringAsScript("foo.calculateChild('Bar', 'if value != nil then return value + 1 else return 1 end')");
            engine.Update(0);
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return foo.bar").ToObject<BigDouble>());
        }

        [Test]
        public void CalculatorCalledEachUpdateCycle()
        {
            engine.Start();
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1, 0);
            engine.Scripting.EvaluateStringAsScript("foo.calculateChild('Bar', 'if value == nil then return 1 else return value + 1 end')");
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(0);
                Assert.AreEqual(new BigDouble(i + 1), engine.Scripting.EvaluateStringAsScript("return foo.bar").ToObject<BigDouble>());
            }
        }

        [Test]
        public void BuiltInMaxReturnsLargestOfTwoNumbers()
        {
            engine.GlobalProperties["foo"] = 1;
            engine.GlobalProperties["bar"] = 2;
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return math.max(foo, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return math.max(1, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return math.max(foo, 2)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return math.max(1, 2)").ToObject<BigDouble>());
        }
        [Test]
        public void BuiltInMinReturnsSmallestOfTwoNumbers()
        {
            engine.GlobalProperties["foo"] = 1;
            engine.GlobalProperties["bar"] = 2;
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return math.min(foo, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return math.min(1, bar)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return math.min(foo, 2)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return math.min(1, 2)").ToObject<BigDouble>());
        }

        [Test]
        public void BuiltInClampReturnsLargestOfValueAndFloor()
        {
            Assert.AreEqual(new BigDouble(15), engine.Scripting.EvaluateStringAsScript("return math.clamp(15, 10, 20)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(10), engine.Scripting.EvaluateStringAsScript("return math.clamp(5, 10, 20)").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(20), engine.Scripting.EvaluateStringAsScript("return math.clamp(25, 10, 20)").ToObject<BigDouble>());
        }

        [Test]
        public void BuiltInPowReturnsPower()
        {
            Assert.AreEqual(new BigDouble(5).Pow(2), engine.Scripting.EvaluateStringAsScript("return math.pow(5, 2)").ToObject<BigDouble>());
        }

        [Test]
        public void StringToBigDoubleParsesTheString()
        {
            Assert.AreEqual(new BigDouble(10), engine.Scripting.EvaluateStringAsScript("return '10'").ToObject<BigDouble>());
            Assert.IsTrue(BigDouble.IsNaN(engine.Scripting.EvaluateStringAsScript("return 'chungus'").ToObject<BigDouble>()));
        }

        [Test]
        public void NilToBigDoubleReturns0()
        {
            Assert.AreEqual(BigDouble.Zero, engine.Scripting.EvaluateStringAsScript("return nil").ToObject<BigDouble>());
        }

        [Test]
        public void UserDataToBigDoubleThrows()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.Scripting.EvaluateStringAsScript("return value", new Dictionary<string, object>()
                {
                    { "value", new TestEntity(engine, 1) }
                }).ToObject<BigDouble>();
            });
        }

        [Test]
        public void ContextCanBeASingleKeyValuePair()
        {
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return value", new KeyValuePair<string, object>("value", true)).Boolean);
        }

        [Test]
        public void DoesntLikeNullScript()
        {
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                engine.Scripting.EvaluateStringAsScript(null);
            });
        }

        [Test]
        public void CanStartATimer()
        {
            engine.Scripting.EvaluateStringAsScript("engine.schedule(1, 'triggered = true')");
            engine.Start();
            engine.Update(1);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void TimeEndsAfterTrigger()
        {
            engine.Scripting.EvaluateStringAsScript("engine.schedule(1, 'triggered = true')");
            engine.Start();
            engine.Update(1);
            engine.GlobalProperties["triggered"] = false;
            engine.Update(1);
            Assert.IsFalse((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void ErrorFunctionInScriptThrows()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.Scripting.EvaluateStringAsScript("error('foo')");
            });
        }

        [Test]
        public void TryingToEvaluateNullForScriptThrows()
        {
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                engine.Scripting.EvaluateStringAsScript(null);
            });
        }

        [Test]
        public void TryingToEvaluateNullObjectThrows()
        {
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                engine.Scripting.Evaluate(null);
            });
        }

        [Test]
        public void TryingToEvaluateWrongTypeThrows()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Scripting.Evaluate(DynValue.Nil);
            });
        }

        [Test]
        public void CallbackReceivesContextDictionary()
        {
            var context = new Dictionary<string, object>();
            engine.Scripting.Evaluate(DynValue.FromObject(null, (Action<IDictionary<string, object>>)(ctx =>
            {
                Assert.IsTrue(ctx is IDictionary<string, object>);
            })), context);
        }

        [Test]
        public void CallbackContextContainsEngineProperty()
        {
            engine.Scripting.Evaluate(DynValue.FromObject(null, (Action<IDictionary<string, object>>)(ctx =>
            {
                Assert.IsTrue(ctx.ContainsKey("engine"));
            })));
        }

        [Test]
        public void TryingToInsertIntoNonTableThrows()
        {
            var table = false;
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Scripting.EvaluateStringAsScript("table.insert(tbl, 'foo')", new Dictionary<string, object>() {
                    { "tbl", table }
                });
            });
        }

        [Test]
        public void NilToBigDoubleIsZero()
        {
            Assert.AreEqual(BigDouble.Zero, ScriptingService.DynValueToBigDouble(DynValue.Nil));
        }

        [Test]
        public void CanConcatenateBigDoubleAndString()
        {
            Assert.AreEqual("1 + 2", engine.Scripting.EvaluateStringAsScript("return num .. ' + 2'", Tuple.Create<string, object>("num", BigDouble.One)).String);
            Assert.AreEqual("2 + 1", engine.Scripting.EvaluateStringAsScript("return '2 + ' .. num", Tuple.Create<string, object>("num", BigDouble.One)).String);
        }
    }
}