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
            Assert.AreEqual(new BigDouble(0), engine.Scripting.EvaluateString("return player.producers[1].quantity").ToObject<BigDouble>());
            engine.Scripting.EvaluateString("player.BuyProducer(1)");
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateString("return player.producers[1].quantity").ToObject<BigDouble>());
        }

        [Test]
        public void TotalIncomeEqualToQuantityTimePerUnitIncome()
        {
            Configure();

            engine.Start();
            engine.GetPlayer().Producers[1].Quantity = 1;
            engine.Update(0f);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateString("return player.producers[1].TotalOutput").ToObject<BigDouble>());
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

            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateString("return player.producers[1].OutputMultiplier").ToObject<BigDouble>());
        }

        [Test]
        public void ProducerCalculatesUnlockStateOnUpdate()
        {
            Configure();

            engine.Start();

            Assert.IsFalse(engine.Scripting.EvaluateString("return player.producers[1].IsUnlocked").Boolean);

            engine.Update(1);

            Assert.IsTrue(engine.Scripting.EvaluateString("return player.producers[1].IsUnlocked").Boolean);
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

        [Test]
        public void CanSetProducerOutputScript()
        {
            module.AddProducer(new Producer(engine, 100, "", 1, "return multiplier"));
            Configure();

            engine.GlobalProperties["multiplier"] = 5;

            engine.GetPlayer().Producers[100].Quantity = 1;

            Assert.AreEqual(new BigDouble(5), engine.GetProducers()[100].TotalOutput);
        }

        [Test]
        public void ModifierCanChangeOutputScript()
        {
            module.AddProducer(new Producer(engine, 100, "", 1, 1));
            module.AddUpgrade(new Upgrade(engine, 200, "", 1, "return true", "return true", new Dictionary<string, Tuple<string, string>>()
            {
                { "producers[100].UnitOutputScript", Tuple.Create<string, string>("'return 100'", null) }
            }));
            Configure();

            engine.GetPlayer().Producers[100].Quantity = 1;
            Assert.AreEqual(new BigDouble(1), engine.GetProducers()[100].TotalOutput);
            
            engine.GetPlayer().AddModifier(engine.GetUpgrades()[200]);
            Assert.AreEqual(new BigDouble(100), engine.GetProducers()[100].TotalOutput);

        }
    }
}