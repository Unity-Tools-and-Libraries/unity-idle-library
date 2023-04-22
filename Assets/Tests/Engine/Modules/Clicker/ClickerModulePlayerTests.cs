using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModulePlayerTests : ClickerModuleTestsBase
    {
        [Test]
        public void BuyingProducerEmitsEvent()
        {
            Configure();

            engine.GetPlayer().GetResource("points").Quantity = 100000;

            engine.Watch(ProducerBoughtEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer().Watch(ProducerBoughtEvent.EventName, "test", "globals.localTriggered = true");
            engine.GetPlayer<ClickerPlayer>().BuyProducer(1);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["localTriggered"]);
        }

        [Test]
        public void BuyingProducerRecalculatesIncome()
        {
            Configure();

            engine.GetPlayer().GetResource("points").Quantity = 100000;

            Assert.AreEqual(new BigDouble(0), engine.GetPlayer().GetResource("points").TotalIncome);
            engine.GetPlayer<ClickerPlayer>().BuyProducer(1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer().GetResource("points").TotalIncome);
        }

        public class UserType
        {
            public IDictionary<string, string> d = new Dictionary<string, string>();
        }

        [Test]
        public void BuyingUpgradeEmitsEvent()
        {
            Configure();

            engine.GetPlayer().GetResource("points").Quantity = 100000;

            engine.Watch(UpgradeBoughtEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer().Watch(UpgradeBoughtEvent.EventName, "test", "globals.localTriggered = true");
            engine.GetPlayer<ClickerPlayer>().BuyUpgrade(2);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["localTriggered"]);
        }

        [Test]
        public void BuyingUpgradeRecalculatesIncome()
        {
            Configure();

            engine.GetPlayer().GetResource("points").Quantity = 100000;

            engine.GetPlayer<ClickerPlayer>().BuyProducer(1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer().GetResource("points").TotalIncome);
            engine.GetPlayer<ClickerPlayer>().BuyUpgrade(2);
            Assert.AreEqual(new BigDouble(2), engine.GetPlayer().GetResource("points").TotalIncome);
        }

        [Test]
        public void UpgradesCanHaveMultipleLevels()
        {
            Configure();

            engine.GetPlayer().GetResource("points").Quantity = 100000;
            engine.GetPlayer<ClickerPlayer>().BuyProducer(1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer().GetResource("points").TotalIncome);

            Assert.AreEqual(new BigDouble(100),
                engine.GetPlayer<ClickerPlayer>().CalculateCost(engine.GetUpgrades()[2L], 1)["points"]);
            engine.GetPlayer<ClickerPlayer>().BuyUpgrade(2);
            Assert.AreEqual(new BigDouble(115),
                engine.GetPlayer<ClickerPlayer>().CalculateCost(engine.GetUpgrades()[2L], 1)["points"]);
            engine.GetPlayer<ClickerPlayer>().BuyUpgrade(2);
            Assert.AreEqual(new BigDouble(4), engine.GetPlayer().GetResource("points").TotalIncome);
        }

        [Test]
        public void UpgradeUnlockExpressionCanReferencePlayerAsTarget()
        {
            module.AddUpgrade(new Upgrade(engine, 3, "", Tuple.Create<string, BigDouble>("points", 1),
                "return target.GetFlag('set')", "return true", new System.Collections.Generic.Dictionary<string, System.Tuple<string, string>>()));

            Configure();

            engine.Start();
            engine.GetPlayer().SetFlag("set");
            engine.Update(0);
            Assert.IsTrue(engine.GetPlayer<ClickerPlayer>().Upgrades[2].IsUnlocked);
        }

        [Test]
        public void UpgradeEnabledExpressionCanReferencePlayerAsTarget()
        {
            module.AddUpgrade(new Upgrade(engine, 3, "", Tuple.Create<string, BigDouble>("points", 1),
                "return true", "return target.GetFlag('set')", new System.Collections.Generic.Dictionary<string, System.Tuple<string, string>>()));

            Configure();

            engine.Start();
            engine.GetPlayer().SetFlag("set");
            engine.Update(0);
            Assert.IsTrue(engine.GetPlayer<ClickerPlayer>().Upgrades[2].IsEnabled);
        }

        [Test]
        public void ProducerUnlockExpressionCanReferencePlayerAsTarget()
        {
            module.AddProducer(new Producer(engine, 3, "", Tuple.Create<string, BigDouble>("points", 1),
                1, "return target.GetFlag('set')", "return true"));

            Configure();

            engine.Start();
            engine.GetPlayer().SetFlag("set");
            engine.Update(0);
            Assert.IsTrue(engine.GetPlayer<ClickerPlayer>().Producers[3].IsUnlocked);
        }

        [Test]
        public void ProducerEnabledExpressionCanReferencePlayerAsTarget()
        {
            module.AddProducer(new Producer(engine, 3, "", Tuple.Create("points", BigDouble.One), 1, "return true", "return target.GetFlag('set')"));

            Configure();

            engine.Start();
            engine.GetPlayer().SetFlag("set");
            engine.Update(0);
            Assert.IsTrue(engine.GetPlayer<ClickerPlayer>().Producers[3].IsEnabled);
        }

        [Test]
        public void CanAffordReturnsTrueIfCostBelowPoints()
        {
            module.AddProducer(new Producer(engine, 3, "", Tuple.Create("points", BigDouble.One), 1, "return true", "return target.GetFlag('set')"));

            Configure();

            engine.Start();
            engine.GetPlayer().GetResource("points").Change(10000);
            Assert.IsTrue(engine.GetPlayer<ClickerPlayer>().CanAfford(engine.GetProducers()[3], 1));
        }

        [Test]
        public void BuyReducesPoints()
        {
            module.AddProducer(new Producer(engine, 3, "", Tuple.Create("points", BigDouble.One), 1, "return true", "return target.GetFlag('set')"));

            Configure();

            engine.Start();
            engine.GetPlayer().GetResource("points").Change(10000);
            engine.GetPlayer<ClickerPlayer>().BuyProducer(3);
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<ClickerPlayer>().Producers[3].Quantity);
            Assert.AreEqual(new BigDouble(9999), engine.GetPlayer().GetResource("points").Quantity);
        }
    }
}