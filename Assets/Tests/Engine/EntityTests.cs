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
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateString("return target.extraProperties.custom", new Dictionary<string, object>() {
                { "target", entity }
            }).ToObject<BigDouble>());
            engine.Scripting.EvaluateString("target.extraProperties.custom = 2", new Dictionary<string, object>()
            {
                { "target", entity }
            });
            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateString("return target.extraProperties.custom", new Dictionary<string, object>() {
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
    }
}