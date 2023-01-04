using BreakInfinity;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class EntityTests : TestsRequiringEngine
    {
        [Test]
        public void EntityCanHaveArbitraryCustomPropertiesForScripting()
        {
            var entity = new TestEntity(engine, 1);
            entity.ExtraProperties["custom"] = 1;
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return target.extraProperties.custom", new Dictionary<string, object>() {
                { "target", entity }
            }).ToObject<BigDouble>());
            engine.Scripting.EvaluateStringAsScript("target.extraProperties.custom = 2", new Dictionary<string, object>()
            {
                { "target", entity }
            });
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return target.extraProperties.custom", new Dictionary<string, object>() {
                { "target", entity }
            }).ToObject<BigDouble>());
        }

        [Test]
        public void CalculatingNonExistantPropertyThrows()
        {
            var entity = new TestEntity(engine, 1);
            entity.CalculateChild("bub", "jfiaodjf");
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                entity.Update(engine, 1);
            });
        }

        [Test]
        public void CanCalculateField()
        {
            var entity = new TestEntity(engine, 1);
            entity.CalculateChild("foo", "return 1");
            entity.Update(engine, 0);
            Assert.AreEqual(new BigDouble(1), entity.foo);
        }

        [Test]
        public void CanEmitEventWithDictionaryContext()
        {
            var entity = new TestEntity(engine, 1);

            entity.Watch("event", "", "entity = 1");
            engine.Watch("event", "", "engine = 1");

            entity.Emit("event", new Dictionary<string, object>());

            Assert.AreEqual(BigDouble.One, (BigDouble)engine.GlobalProperties["entity"]);
            Assert.AreEqual(BigDouble.One, (BigDouble)engine.GlobalProperties["engine"]);
        }
    }
}