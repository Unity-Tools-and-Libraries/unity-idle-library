using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Tests.Engine.Scripting;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class EngineTests : TestsRequiringEngine
    {

        [Test]
        public void CanSubscribeToEvents()
        {
            engine.Watch("event", "test", "globals.triggered = true");
            engine.Emit("event", engine);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void SetsDefaultConfigurationProperty()
        {
            Assert.NotNull(engine.GetConfiguration());
        }
        
        [Test]
        public void SetsDefaultDefinitionsProperty()
        {
            Assert.NotNull(engine.GetDefinitions());
        }

        [Test]
        public void GetMissingPropertyReturnsNull()
        {
            Assert.AreEqual(BigDouble.Zero, engine.GetProperty<BigDouble>("foo"));
        }

        [Test]
        public void CanGetPropertyThroughFields()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            (engine.GlobalProperties["foo"] as TestEntity).Bar = new BigDouble(1);
            Assert.AreEqual(new BigDouble(1), engine.GetProperty<BigDouble>("foo.Bar"));
        }

        [Test]
        public void CanGetPropertyWithBracketOperator()
        {
            engine.GlobalProperties["foo"] = new Dictionary<string, object>();
            (engine.GlobalProperties["foo"] as IDictionary<string, object>)["bar"] = new BigDouble(1);
            Assert.AreEqual(new BigDouble(1), engine.GetProperty<BigDouble>("foo[bar]"));
        }

        [Test]
        public void GetPropertyThatDoesntExistReturnsNull()
        {
            engine.GlobalProperties["foo"] = new TestEntity(engine, 1);
            (engine.GlobalProperties["foo"] as TestEntity).Bar = new BigDouble(1);
            Assert.AreEqual(default(BigDouble), engine.GetProperty<BigDouble>("foo.fum"));
        }

        [Test]
        public void CannotAddModuleAfterStart()
        {
            engine.Start();
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.AddModule(new RpgModule());
            });
        }

        [Test]
        public void CanTraverseFieldOfAnObject()
        {
            engine.GlobalProperties["entity"] = new TestEntityWithField(engine, 1);
            Assert.AreEqual(1, engine.GetProperty("entity.foo"));
        }
        
        [Test]
        public void CanCalculateGlobalProperties()
        {
            engine.CalculateProperty("global", "return 1");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(1, engine.GlobalProperties["global"]);
        }

        [Test]
        public void CanClearCalculatorToStopCalculating()
        {
            engine.CalculateProperty("global", "if value == nil then return deltaTime else return value + deltaTime end");
            engine.Start();
            engine.Update(1.5f);
            Assert.AreEqual(1.5, engine.GlobalProperties["global"]);
            engine.CalculateProperty("global", null);
            engine.Update(1f);
            Assert.AreEqual(1.5, engine.GlobalProperties["global"]);
        }

        [Test]
        public void TraversingPropertiesGoesOverAnnoatedUserDataProperties()
        {
            UserData.RegisterType<TestEntityWithField>();
            engine.GlobalProperties["foo"] = new TestEntityWithField(engine, 1);
            (engine.GlobalProperties["foo"] as TestEntityWithField).child = new TestEntityWithField(engine, 2, 2);
            Assert.IsTrue(engine.TraverseObjectGraph().Any(x =>
            {
                object o = x.ToObject();
                return o is TestEntityWithField te && te.foo == 2;
            }));
        }

        //[Test]
        public void NoTwoEntitiesCanHaveTheSameIds()
        {
            new TestEntityWithField(engine, 1);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                new TestEntityWithField(engine, 1);
            });
        }

        [Test]
        public void CanScheduleScriptToCall()
        {
            engine.Schedule(.1, "globals.triggered = true");

            engine.Start();

            engine.Update(1);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        public class TestEntityWithField : Entity
        {
            public TestEntityWithField(IdleEngine engine, long id, int foo = 1) : base(engine, id)
            {
                this.foo = foo;
            }

            public int foo;
            [TraversableFieldOrProperty]
            public TestEntityWithField child;
        }
    }
}