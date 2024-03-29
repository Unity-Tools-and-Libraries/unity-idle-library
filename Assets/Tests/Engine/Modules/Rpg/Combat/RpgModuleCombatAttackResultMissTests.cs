using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.Combat.Attack
{
    public class RpgModuleCombatAttackResultMissTests : RpgModuleTestsBase
    {
        [Test]
        public void TheDefaultPlayerAttackScriptCanMiss()
        {
            Configure();

            random.SetNextValues(0, 99, 999);

            engine.StartEncounter();

            var result = engine.MakeAttack(engine.GetPlayer<RpgPlayer>().Character, engine.GetCurrentEncounter().Creatures[0]);

            Assert.AreEqual("miss", result.Description);
        }

        [Test]
        public void AbilitiesOnTheAttackerCanModifyAMissedAttack()
            {
            random.SetNextValues(1, 999, 0, 1, 1);
            rpgModule.AddAbility(new CharacterAbility.Builder()
                .WithEventTrigger("IsAttacking", "attackResult.isHit = true; attackResult.description = 'hit'; table.insert(attackResult.DamageToDefender, attackResult.OriginalDamageToDefender)")
                .Build(engine, 5));

            Configure();

            engine.GetPlayer<RpgPlayer>().Character.AddAbility(engine.GetAbilities()[5]);

            var defender = new RpgCharacter(engine, 7);

            var result = engine.MakeAttack(engine.GetPlayer<RpgPlayer>().Character, defender);

            Assert.IsTrue(result.IsHit);
            Assert.AreEqual(new List<Tuple<BigDouble, RpgCharacter>>()
            {
                Tuple.Create(new BigDouble(20), engine.GetPlayer<RpgPlayer>().Character)
            }, result.DamageToDefender);
        }

        [Test]
        public void AbilitiesOnTheDefenderCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 999, 0, 1, 1);
            rpgModule.AddAbility(new CharacterAbility.Builder().WithEventTrigger("IsBeingAttacked", "attackResult.isHit = true; attackResult.description = 'hit'; attackResult.DamageAttacker(10, attacker);")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddAbility(engine.GetAbilities()[5]);

            var result = engine.MakeAttack(engine.GetPlayer<RpgPlayer>().Character, defender);

            Assert.IsTrue(result.IsHit);
            Assert.AreEqual("hit", result.Description);
            Assert.AreEqual(new List<Tuple<BigDouble, RpgCharacter>>() {
                Tuple.Create(new BigDouble(20), engine.GetPlayer<RpgPlayer>().Character)
            }, result.DamageToDefender);

            Assert.AreEqual(new List<Tuple<BigDouble, RpgCharacter>>() {
                Tuple.Create(new BigDouble(10), engine.GetPlayer<RpgPlayer>().Character)
            }, result.DamageToAttacker);
        }
    }
}