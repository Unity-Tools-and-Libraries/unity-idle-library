using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class PlayerTests : TestsRequiringEngine
    {
        [Test]
        public void CanGetAResource()
        {
            engine.GlobalProperties["player"] = new Player(engine, 1, new Dictionary<string, BreakInfinity.BigDouble>()
            {
                { "points", 1 }
            });
            var resource = engine.GetPlayer<Player>().GetResource("points");
            Assert.AreEqual(BigDouble.One, resource.Quantity);
        }

        [Test]
        public void GettingNonExistantResourceThrows()
        {
            engine.GlobalProperties["player"] = new Player(engine, 1, new Dictionary<string, BreakInfinity.BigDouble>()
            {
                { "points", 1 }
            });
            Assert.Throws<InvalidOperationException>(() => {
                engine.GetPlayer<Player>().GetResource("foobar");
                });
        }
    }
}