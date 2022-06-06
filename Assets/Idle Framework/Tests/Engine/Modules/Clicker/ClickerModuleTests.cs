using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModuleTests : RequiresEngineTests
    {
        [Test]
        public void AddsResourceType()
        {
            engine.AddModule(new ClickerModule());
            Assert.NotNull(engine.GetDefinition<ResourceDefinition>("resource", "points"));
        }

        [Test]
        public void SetsDefaultCostScalingProperty()
        {
            engine.AddModule(new ClickerModule());
            Assert.AreEqual(new BigDouble(1.15), engine.GetProperty(ClickerModule.DefaultProperties.PRODUCER_COST_SCALE_FACTOR).ValueAsNumber());
        }
    }
}