using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Tests.Engine.Scripting;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class EngineTests : TestsRequiringEngine
    {

        [Test]
        public void CanSubscribeToEvents()
        {
            engine.Watch("event", "test", "triggered = true");
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
        public void CanGetPropertyThroughFields()
        {
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            (engine.GlobalProperties["foo"] as TestCustomType).Bar = new BigDouble(1);
            Assert.AreEqual(new BigDouble(1), engine.GetProperty<BigDouble>("foo.Bar"));
        }

        [Test]
        public void GetPropertyThatDoesntExistReturnsNull()
        {
            engine.GlobalProperties["foo"] = new TestCustomType(engine);
            (engine.GlobalProperties["foo"] as TestCustomType).Bar = new BigDouble(1);
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
            engine.GlobalProperties["entity"] = new TestEntityWithField(engine);
            Assert.AreEqual(1, engine.GetProperty("entity.foo"));
        }

        public class TestEntityWithField : Entity
        {
            public int foo = 1;
            public TestEntityWithField(IdleEngine engine) : base(engine)
            {
            }
        }
    }
}