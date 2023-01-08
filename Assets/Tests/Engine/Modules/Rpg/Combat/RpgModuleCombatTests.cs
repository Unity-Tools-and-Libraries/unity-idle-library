using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCombatTests : RpgModuleTestsBase
    {

        [Test]
        public void AbilitiesOnTheAttackerCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 1, 1, 1, 1);
            rpgModule.AddAbility(new CharacterAbility.Builder()
                .WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            engine.GetPlayer<RpgCharacter>().AddAbility(engine.GetAbilities()[5]);

            var defender = new RpgCharacter(engine, 7);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsFalse(result.IsHit);
            Assert.AreEqual(0, result.DamageToDefender.Count);
        }

        [Test]
        public void AbilitiesOnTheDefenderCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 1, 1, 1, 1);
            rpgModule.AddAbility(new CharacterAbility.Builder().WithEventTrigger("IsBeingAttacked", "attack.isHit = false; attack.description = 'miss'; attack.clearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddAbility(engine.GetAbilities()[5]);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsFalse(result.IsHit);
            Assert.AreEqual(0, result.DamageToDefender.Count);
        }
        
        [Test]
        public void RemovingAbilityRemovesTriggers()
        {
            random.SetNextValues(1, 1, 0, 1, 1);
            rpgModule.AddAbility(new CharacterAbility.Builder().WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.clearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddAbility(engine.GetAbilities()[5]);
            defender.RemoveAbility(engine.GetAbilities()[5]);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsTrue(result.IsHit);
        }

        [Test]
        public void StatusesOnTheAttackerCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 1, 1, 1, 1);
            rpgModule.AddStatus(new CharacterStatus.Builder().WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            engine.GetPlayer<RpgCharacter>().AddStatus(engine.GetStatuses()[5], 1);

            var defender = new RpgCharacter(engine, 7);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsFalse(result.IsHit);
            Assert.AreEqual(0 , result.DamageToDefender.Count);
        }

        [Test]
        public void StatusesOnTheDefenderCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 1, 1, 1, 1);
            rpgModule.AddStatus(new CharacterStatus.Builder().WithEventTrigger("IsBeingAttacked", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddStatus(engine.GetStatuses()[5], 1);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsFalse(result.IsHit);
            Assert.AreEqual(0, result.DamageToDefender.Count);
        }

        [Test]
        public void RemovingStatusRemovesTriggers()
        {
            random.SetNextValues(1, 1, 0, 1, 1);
            rpgModule.AddStatus(new CharacterStatus.Builder().WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddStatus(engine.GetStatuses()[5], 1);
            defender.RemoveStatus(engine.GetStatuses()[5]);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsTrue(result.IsHit);
        }

        [Test]
        public void ItemsOnTheAttackerCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 1, 1, 1, 1);
            rpgModule.AddItem(new CharacterItem.Builder().WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            engine.GetPlayer<RpgCharacter>().AddItem(engine.GetItems()[5]);

            var defender = new RpgCharacter(engine, 7);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsFalse(result.IsHit);
            Assert.AreEqual(0, result.DamageToDefender.Count);
        }

        [Test]
        public void ItemsOnTheDefenderCanModifyAnAttackBeingMade()
        {
            random.SetNextValues(1, 1, 1, 1, 1);
            rpgModule.AddItem(new CharacterItem.Builder().WithEventTrigger("IsBeingAttacked", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage()")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddItem(engine.GetItems()[5]);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsFalse(result.IsHit);
            Assert.AreEqual(0, result.DamageToDefender.Count);
        }

        [Test]
        public void RemovingItemRemovesTrigger()
        {
            random.SetNextValues(1, 1, 0, 1, 1);
            rpgModule.AddItem(new CharacterItem.Builder().WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.damageToDefender = 0")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.AddItem(engine.GetItems()[5]);
            defender.RemoveItem(engine.GetItems()[5]);

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.IsTrue(result.IsHit);
        }

        [Test]
        public void AttackThatDealsLessThanMinimumDamageIsMinimum()
        {
            random.SetNextValues(1, 1, 0, 1, 1);
            rpgModule.AddItem(new CharacterItem.Builder().WithEventTrigger("IsAttacking", "attack.isHit = false; attack.description = 'miss'; attack.ClearDefenderDamage(); attack.DamageDefender(0, attacker);")
                .Build(engine, 5));

            Configure();

            var defender = new RpgCharacter(engine, 7);
            defender.Defense.BaseValue = 100_000;

            var result = engine.MakeAttack(engine.GetPlayer<RpgCharacter>(), defender);

            Assert.AreEqual(new BigDouble(1), result.DamageToDefender[0].Item1);
        }

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