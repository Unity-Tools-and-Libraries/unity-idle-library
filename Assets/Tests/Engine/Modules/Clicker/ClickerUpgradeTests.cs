using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerUpgradeTests : ClickerModuleTestsBase
    {
        [Test]
        public void BuyingAnUpgradeAppliesIt()
        {
            var upgrade = new Upgrade(engine, engine.GetNextAvailableId(), "", 0, "return true", "return true", new Dictionary<string, System.Tuple<string, string>>());
            module.AddUpgrade(upgrade);

            Configure();

            engine.Scripting.EvaluateStringAsScript(string.Format("globals.player.BuyUpgrade({0})", upgrade.Id));
            Assert.IsTrue(engine.GetPlayer().GetModifiers().Contains(upgrade.Id));

        }

        [Test]
        public void UpgradeCalculatesUnlockStateOnUpdate()
        {
            Configure();

            engine.Start();

            Assert.IsFalse(engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].IsUnlocked").Boolean);

            engine.Update(1);

            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.player.producers[1].IsUnlocked").Boolean);
        }

        [Test]
        public void WhenUpgradeUnlockStateChangesEventEmitted()
        {
            var upgrade = new Upgrade(engine, engine.GetNextAvailableId(), "", 0, "return true", "return true", new Dictionary<string, System.Tuple<string, string>>());
            module.AddUpgrade(upgrade);

            Configure();

            engine.Start();

            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("globaltriggered"));

            engine.GetPlayer<ClickerPlayer>().Upgrades[upgrade.Id].Watch(IsUnlockedChangeEvent.EventName, "test", "globals.triggered = true");
            engine.Watch(IsUnlockedChangeEvent.EventName, "test", "globals.globaltriggered = true");

            engine.Update(1);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["globaltriggered"]);
        }

        [Test]
        public void WhenAnUpgradeReachesMaxLevelEventEmitted()
        {
            var upgrade = new Upgrade(engine, engine.GetNextAvailableId(), "", 0, "return true", "return true", new Dictionary<string, System.Tuple<string, string>>());
            module.AddUpgrade(upgrade);

            Configure();

            engine.Start();

            Assert.IsFalse(engine.GlobalProperties.ContainsKey("triggered"));
            Assert.IsFalse(engine.GlobalProperties.ContainsKey("globaltriggered"));

            engine.GetPlayer<ClickerPlayer>().Upgrades[upgrade.Id].Watch(MaxLevelReachedEvent.EventName, "test", "globals.localtriggered = true");
            engine.GetPlayer<ClickerPlayer>().Watch(MaxLevelReachedEvent.EventName, "test", "globals.triggered = true");
            engine.Watch(MaxLevelReachedEvent.EventName, "test", "globals.globaltriggered = true");

            engine.GetPlayer<ClickerPlayer>().BuyUpgrade(upgrade.Id);

            Assert.IsTrue((bool)engine.GlobalProperties["localtriggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
            Assert.IsTrue((bool)engine.GlobalProperties["globaltriggered"]);
        }
    }
}