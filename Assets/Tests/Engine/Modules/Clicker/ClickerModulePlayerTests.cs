using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker;
using NUnit.Framework;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModulePlayerTests : ClickerModuleTestsBase
    {
        [Test]
        public void BuyingProducerEmitsEvent()
        {
            Configure();

            engine.GetPlayer().Points.Quantity = 100000;

            engine.Watch(ProducerBoughtEvent.EventName, "test", "triggered = true");
            engine.GetPlayer().Watch(ProducerBoughtEvent.EventName, "test", "localTriggered = true");
            engine.GetPlayer().BuyProducer(1);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["localTriggered"]);
        }

        [Test]
        public void BuyingUpgradeEmitsEvent()
        {
            Configure();

            engine.GetPlayer().Points.Quantity = 100000;

            engine.Watch(UpgradeBoughtEvent.EventName, "test", "triggered = true");
            engine.GetPlayer().Watch(UpgradeBoughtEvent.EventName, "test", "localTriggered = true");
            engine.GetPlayer().BuyUpgrade(2);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["localTriggered"]);
        }
    }
}