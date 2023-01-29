using System;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using MoonSharp.Interpreter;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleAdvancementTests : RpgModuleTestsBase
    {
        [Test]
        public void CanSpecifyBuyingAttributeAdvancement()
        {
            rpgModule.ConfigureAttributeAdvancement()
                .DefineUpgradeableAttribute("damage", new Dictionary<string, DynValue>()
                {
                    { "xp", DynValue.FromObject(null, (Func<BigDouble,BigDouble>)(level => {
                        return new BigDouble(1.5).Pow(level - 1);
                    }))
                    }
                }) ;
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().Damage.BaseValue = 1;

            Assert.AreEqual(new BigDouble(1.5), engine.GetPlayerCharacter<RpgCharacter>()
                .GetCostToBuyAttribute("damage")["xp"]);
        }

        [Test]
        public void CanBuyDefinedAttributeAdvancement()
        {
            rpgModule.ConfigureAttributeAdvancement()
                .DefineUpgradeableAttribute("damage", new Dictionary<string, DynValue>()
                {
                    { "xp", DynValue.FromObject(null, (Func<BigDouble,BigDouble>)(level => new BigDouble(1.5).Pow(level - 1))) }
                });
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().Damage.BaseValue = 1;

            Assert.AreEqual(new BigDouble(1.5), engine.GetPlayerCharacter<RpgCharacter>().GetCostToBuyAttribute("damage")["xp"]);
        }
    }
}