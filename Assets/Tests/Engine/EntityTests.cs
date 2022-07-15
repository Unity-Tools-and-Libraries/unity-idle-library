using BreakInfinity;
using NUnit.Framework;
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
            var entity = new TestEntity(engine);
            entity.ExtraProperties["custom"] = 1;
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return target.extraProperties.custom", new Dictionary<string, object>() {
                { "target", entity }
            }).ToObject<BigDouble>());
            engine.Scripting.Evaluate("target.extraProperties.custom = 2", new Dictionary<string, object>()
            {
                { "target", entity }
            });
            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return target.extraProperties.custom", new Dictionary<string, object>() {
                { "target", entity }
            }).ToObject<BigDouble>());
        }
    }
}