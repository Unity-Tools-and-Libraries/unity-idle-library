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

            engine.GetPlayer().GetResource("points").Quantity = 10000;
            Assert.AreEqual(new BigDouble(0), engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].quantity").ToObject<BigDouble>());
            engine.Scripting.EvaluateStringAsScript("globals.player.BuyProducer(1)");
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].quantity").ToObject<BigDouble>());
        }

        [Test]
        public void ProducerCostToBuyScalesByQuantityAlreadyOwned()
        {
            Configure();

            engine.GetPlayer().GetResource("points").Quantity = 10000;
            for (int i = 0; i < 10; i++)
            {
                engine.GetPlayer<ClickerPlayer>().Producers[1].Quantity = i;
                Assert.AreEqual(new BigDouble(1) * BigDouble.Pow(1.15, i), engine.GetPlayer<ClickerPlayer>().CalculateCost(engine.GetProducers()[1], 1)["points"]);
            }
        }

        [Test]
        public void TotalIncomeEqualToQuantityTimePerUnitIncome()
        {
            Configure();

            engine.Start();
            engine.GetPlayer<ClickerPlayer>().Producers[1].Quantity = 1;
            engine.Update(0f);
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].TotalOutput").ToObject<BigDouble>());
        }

        [Test]
        public void UpgradesCanModifyProducers()
        {
            Configure();

            var upgrade = new Upgrade(engine, engine.GetNextAvailableId(), "", Tuple.Create("points", new BigDouble(1)), "return true", "return true", new Dictionary<string, System.Tuple<string, string>>()
            {
                { "producers[1].OutputMultiplier", Tuple.Create("value * 2", "value / 2") }
            });
            engine.GetPlayer().AddModifier(upgrade);

            Assert.AreEqual(new BigDouble(2), engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].OutputMultiplier").ToObject<BigDouble>());
        }

        [Test]
        public void ProducerCalculatesUnlockStateOnUpdate()
        {
            Configure();

            engine.Start();

            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].IsUnlocked").Boolean);

            engine.Update(1);

            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].IsUnlocked").Boolean);
        }

        [Test]
        public void WhenProducerUnlockStateChangesEventEmitted()
        {
            Configure();

            engine.Start();

            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("globaltriggered"));

            engine.GetPlayer<ClickerPlayer>().Producers[1].Watch(IsUnlockedChangeEvent.EventName, "test", "globals.triggered = true");
            engine.Watch(IsUnlockedChangeEvent.EventName, "test", "globals.globaltriggered = true");

            engine.Update(1);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["globaltriggered"]);
        }

        [Test]
        public void CanSetProducerOutputScript()
        {
            module.AddProducer(new Producer(engine, 100, "", Tuple.Create<string, BigDouble>("points", BigDouble.One), "return globals.multiplier"));
            Configure();

            engine.GlobalProperties["multiplier"] = 5;

            engine.GetPlayer<ClickerPlayer>().Producers[100].Quantity = 1;

            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<ClickerPlayer>().Producers[100].CalculateOutput(engine));
        }

        [Test]
        public void ModifierCanChangeOutputScript()
        {
            module.AddProducer(new Producer(engine, 100, "", Tuple.Create("points", BigDouble.One), 1));
            module.AddUpgrade(new Upgrade(engine, 200, "", Tuple.Create("points", BigDouble.One), "return true", "return true", new Dictionary<string, Tuple<string, string>>()
            {
                { "producers[100].UnitOutputScript", Tuple.Create<string, string>("'return 100'", null) }
            }));
            Configure();

            engine.GetPlayer<ClickerPlayer>().Producers[100].Quantity = 1;
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<ClickerPlayer>().Producers[100].CalculateOutput(engine));
            
            engine.GetPlayer().AddModifier(engine.GetUpgrades()[200]);
            Assert.AreEqual(new BigDouble(100), engine.GetPlayer<ClickerPlayer>().Producers[100].CalculateOutput(engine));

        }

        [Test]
        public void CostToBuyWithNoneOwnedCostsBase()
        {
            module.AddProducer(new Producer(engine, 100, "", Tuple.Create("points", new BigDouble(100)), 1));
            Configure();

            Assert.AreEqual(new BigDouble(100), engine.CalculatePurchaseCost(engine.GetProducers()[100], 0, 1)["points"]);
        }

        [Test]
        public void CostToBuyWithSomeOwnedScalesUp()
        {
            module.AddProducer(new Producer(engine, 100, "", Tuple.Create("points", "return 100 * math.pow(1.15, level)"), 1));
            Configure();

            Assert.AreEqual(new BigDouble(115), engine.CalculatePurchaseCost(engine.GetProducers()[100], 1, 1)["points"]);
        }
    }
}