using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Scripting
{
    public class ScriptingTests : TestsRequiringEngine
    {
        [SetUp]
        public void setup()
        {
            base.InitializeEngine();

            UserData.RegisterType<TestCustomType>();
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
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
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
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            engine.Scripting.Evaluate("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.Evaluate("foo.stopWatching('event', 'test')");
            engine.Scripting.Evaluate("foo.emit('event')");
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
        }

        [Test]
        public void CanBroadcastEvent()
        {
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            engine.Scripting.Evaluate("foo.watch('event', 'test', 'triggered = true')");
            engine.Scripting.Evaluate("foo.emit('event')");
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanCalculateValue()
        {
            engine.Start();
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            engine.Scripting.Evaluate("foo.calculateChild('Bar', 'if value != nil then return value + 1 else return 1 end')");
            engine.Update(0);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return foo.bar").ToObject<BigDouble>());
        }

        [Test]
        public void CalculatorCalledEachUpdateCycle()
        {

            engine.Start();
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            engine.Scripting.Evaluate("foo.calculateChild('Bar', 'if value == nil then return 1 else return value + 1 end')");
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(0);
                Assert.AreEqual(new BigDouble(i), engine.Scripting.Evaluate("return foo.bar").ToObject<BigDouble>());
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
    }

    public class TestCustomType : Entity
    {
        public TestCustomType(IdleEngine engine) : base(engine)
        {
        }

        public object Bar{ get; set;}
        public object Baz { get; set; }

        public void AddCustomModifier(CustomModifier modifier)
        {
            this.AddModifier(modifier);
        }

        public override bool Equals(object obj)
        {
            return obj is TestCustomType type &&
                   EqualityComparer<object>.Default.Equals(Bar, type.Bar) &&
                   EqualityComparer<object>.Default.Equals(Baz, type.Baz);
        }
    }

    public class CustomModifier : EntityModifier<Entity>
    {
        public CustomModifier(IdleEngine engine, Dictionary<string, Tuple<string, string>> modifications) : base(engine, 1, modifications)
        {
        }

        public override void Apply(framework.Engine.Entity target)
        {
            throw new System.NotImplementedException();
        }

        public override void Unapply(framework.Engine.Entity target)
        {
            throw new System.NotImplementedException();
        }
    }
}