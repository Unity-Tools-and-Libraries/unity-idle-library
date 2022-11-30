using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCharacterItemsTests : RpgModuleTestsBase
    {
        [Test]
        public void ItemsCanHaveDescriptions()
        {
            CharacterItem item = new CharacterItem.Builder()
                .WithDescription("foobar")
                .Build(engine, 1);
            Assert.AreEqual("foobar", item.Description);
        }
    }
}