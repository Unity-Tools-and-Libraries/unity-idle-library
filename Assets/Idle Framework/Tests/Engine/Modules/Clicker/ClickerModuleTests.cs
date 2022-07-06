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

        [Test]
        public void DoClickAddsPointsEqualToClickIncome()
        {
            engine.AddModule(new ClickerModule());
            Assert.AreEqual(new BigDouble(0), engine.GetProperty("points.quantity").ValueAsNumber());
            engine.EvaluateExpression("DoClick()");
            Assert.AreEqual(new BigDouble(1), engine.GetProperty("points.quantity").ValueAsNumber());
            engine.GetProperty("points.click_income").Set(10);
            engine.EvaluateExpression("DoClick()");
            Assert.AreEqual(new BigDouble(11), engine.GetProperty("points.quantity").ValueAsNumber());
        }
    }
}