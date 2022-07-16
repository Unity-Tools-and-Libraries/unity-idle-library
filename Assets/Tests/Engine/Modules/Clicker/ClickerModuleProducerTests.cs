using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
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
            engine.GetPlayer().Producers[1].Quantity = 1;
            engine.Update(0f);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.Evaluate("return player.producers[1].TotalOutput").ToObject<BigDouble>());
        }

        [Test]
        public void UpgradesCanModifyProducers()
        {
            Configure();

            var upgrade = new Upgrade(engine, engine.GetNextAvailableId(), "", 1, "return true", "return true", new Dictionary<string, System.Tuple<string, string>>()
            {
                { "producers[1].OutputMultiplier", Tuple.Create("value * 2", "value / 2") }
            });
            engine.GetPlayer().AddModifier(upgrade);

            Assert.AreEqual(new BigDouble(2), engine.Scripting.Evaluate("return player.producers[1].OutputMultiplier").ToObject<BigDouble>());
        }

        [Test]
        public void ProducerCalculatesUnlockStateOnUpdate()
        {
            Configure();

            engine.Start();

            Assert.IsFalse(engine.Scripting.Evaluate("return player.producers[1].IsUnlocked").Boolean);

            engine.Update(1);

            Assert.IsTrue(engine.Scripting.Evaluate("return player.producers[1].IsUnlocked").Boolean);
        }

        [Test]
        public void WhenProducerUnlockStateChangesEventEmitted()
        {
            Configure();

            engine.Start();

            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("globaltriggered"));

            engine.GetPlayer().Producers[1].Watch(IsUnlockedChangeEvent.EventName, "test", "triggered = true");
            engine.Watch(IsUnlockedChangeEvent.EventName, "test", "globaltriggered = true");

            engine.Update(1);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["globaltriggered"]);
        }
    }
}