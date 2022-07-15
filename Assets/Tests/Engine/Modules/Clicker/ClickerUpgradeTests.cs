using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
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
            var upgrade = new Upgrade(engine, 1, "", 0, "return true", "return true", new Dictionary<string, System.Tuple<string, string>>());
            module.AddUpgrade(upgrade);

            Configure();

            engine.Scripting.Evaluate("player.BuyUpgrade(1)");
            Assert.IsTrue(engine.GetPlayer().GetModifiers().Contains(upgrade.Id));

        }
    }
}