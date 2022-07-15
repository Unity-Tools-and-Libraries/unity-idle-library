using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker {
    public class ClickerModuleProducerTests : ClickerModuleTestsBase
    {
        [Test]
        public void BuyingProducerIncreasesQuantity()
        {
            Configure();

            engine.GetPlayer().Points.Quantity = 10000;
            Assert.AreEqual(new BigDouble(0), engine.Scripting.Evaluate("return player.producers[1].quantity").ToObject<BigDouble>());
            engine.Scripting.Evaluate("player.BuyProducer(1)");
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return player.producers[1].quantity").ToObject<BigDouble>());
        }

        [Test]
        public void TotalIncomeEqualToQuantityTimePerUnitIncome()
        {
            Configure();

            engine.Start();
            engine.GetProducers()[1].Quantity = 1;
            engine.Update(0f);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return player.producers[1].TotalOutput").ToObject<BigDouble>());
        }

        [Test]
        public void UpgradesCanModifyProducers()
        {
            Configure();

            var upgrade = new Upgrade(engine, 1, "", 1, "return true", "return true", new Dictionary<string, System.Tuple<string, string>>()
            {
                { "producers[1].OutputMultiplier", Tuple.Create("value * 2", "value / 2") }
            });
            engine.GetPlayer().AddModifier(upgrade);

            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return player.producers[1].OutputMultiplier").ToObject<BigDouble>());
        }
    }
}