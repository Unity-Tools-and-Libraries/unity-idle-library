using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using NUnit.Framework;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class PlayerTests : TestsRequiringEngine
    {
        [Test]
        public void CanGetAResource()
        {
            engine.GlobalProperties["player"] = new Player(engine, 1)
            {
                Resources = new Dictionary<string, ResourceHolder>()
                {
                    { "points", new ResourceHolder(1) }
                }
            };
            var resource = engine.GetPlayer<Player>().GetResource("points");
            Assert.AreEqual(BigDouble.One, resource.Quantity);
        }

        [Test]
        public void GettingNonExistantResourceThrows()
        {
            engine.GlobalProperties["player"] = new Player(engine, 1)
            {
                Resources = new Dictionary<string, ResourceHolder>(){
                    { "points", new ResourceHolder(1) }
                }
            };
            Assert.Throws<InvalidOperationException>(() => {
                engine.GetPlayer<Player>().GetResource("foobar");
                });
        }
    }
}