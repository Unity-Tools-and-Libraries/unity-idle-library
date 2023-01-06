using System;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.Combat.Attack
{
    public class RpgModuleCombatTests : RpgModuleTestsBase
    {
        [Test]
        public void CharacterDamageModifiesAttackDamage()
        {
            Configure();

            random.SetNextValues(0, 0, 0, 0, 0, 0);

            engine.StartEncounter();
            Assert.AreEqual(new List<Tuple<BigDouble, RpgCharacter>>()
            {
                Tuple.Create(new BigDouble(6), engine.GetPlayer<RpgCharacter>())
            },
                engine.MakeAttack(engine.GetPlayer<RpgCharacter>(),
                engine.GetCurrentEncounter().Creatures[0]).DamageToDefender);

            engine.GetPlayer<RpgCharacter>().Damage.ChangePerLevel = 1;
            engine.GetPlayer<RpgCharacter>().Damage.Level = 2;

            engine.StartEncounter();
            Assert.AreEqual(new List<Tuple<BigDouble, RpgCharacter>>()
            {
                Tuple.Create(new BigDouble(7), engine.GetPlayer<RpgCharacter>())
            },
                engine.MakeAttack(engine.GetPlayer<RpgCharacter>(),
                engine.GetCurrentEncounter().Creatures[0]).DamageToDefender);
        }
    }
}