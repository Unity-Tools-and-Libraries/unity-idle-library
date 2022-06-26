using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.RpgModuleCombatTests;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.Abilities
{
    public class RpgModuleAbilitiesInCombatTests : RequiresEngineTests
    {
        private Character attacker, defender;
        private RiggedRandom rng;

        [SetUp]
        public void Setup()
        {
            rng = new RiggedRandom();
            base.InitializeEngine();
            var module = new RpgModule();
            module.AddCreature(new CreatureDefinition.Builder().Build("1"));
            module.AddEncounter(new EncounterDefinition("1", Tuple.Create("1", 1)));
            engine.AddModule(module);
            engine.CreateProperty("configuration.action_meter_required_to_act", 2);
            engine.OverrideRandomNumberGenerator(rng);

            attacker = engine.CreateProperty("attacker", new Dictionary<string, ValueContainer>() {
                { "abilities", engine.CreateValueContainer(new List<ValueContainer>()) }
            }, updater: "CharacterUpdateMethod").AsCharacter();
            defender = engine.CreateProperty("defender", new Dictionary<string, ValueContainer>() { 
                { "abilities", engine.CreateValueContainer(new List<ValueContainer>()) } 
            }, updater: "CharacterUpdateMethod").AsCharacter();

            defender.MaximumHealth = defender.CurrentHealth = 10;
            attacker.Damage = 1;

            var onHitting = new AbilityDefinition("on-attack", "", null, events: new Dictionary<string, List<string>>()
            {
                { AttackHitEvent.EventName, new List<string>() { "attacker.hit = true" } }
            });
            var onMissing = new AbilityDefinition("on-miss", "", null, events: new Dictionary<string, List<string>>()
            {
                { AttackMissEvent.EventName, new List<string>() { "attacker.missed = true" } }
            });
            var onBeingHit = new AbilityDefinition("on-attack", "", null, events: new Dictionary<string, List<string>>()
            {
                { HitByAttackEvent.EventName, new List<string>() { "defender.was_hit = true" } }
            });
            var onBeingMissed = new AbilityDefinition("on-missed", "", null, events: new Dictionary<string, List<string>>()
            {
                { MissedByAttackEvent.EventName, new List<string>() { "defender.was_missed = true" } }
            });
            attacker.AddAbility(onHitting);
            attacker.AddAbility(onMissing);

            defender.AddAbility(onBeingHit);
            defender.AddAbility(onBeingMissed);

            engine.GetProperty("action_phase").Set("combat");

            engine.OverrideRandomNumberGenerator(rng);

            engine.Start();
        }

        [Test]
        public void AbilityCanTriggerOnHittingWithAttack()
        {
            rng.SetNextValue(1);
            rng.SetNextValue(100);
            engine.MakeAttack(attacker, defender);
            Assert.IsTrue(attacker.GetProperty("hit").ValueAsBool());
        }

        [Test]
        public void AbilityCanTriggerOnMissingingWithAttack()
        {
            rng.SetNextValue(100);
            rng.SetNextValue(100);
            engine.MakeAttack(attacker, defender);
            Assert.IsTrue(attacker.GetProperty("missed").ValueAsBool());
        }

        [Test]
        public void AbilityCanTriggerOnBeingMissedWithAttack()
        {
            rng.SetNextValue(100);
            engine.MakeAttack(attacker, defender);
            Assert.IsTrue(defender.GetProperty("was_missed").ValueAsBool());
        }

        [Test]
        public void AbilityCanTriggerOnBeingHitWithAttack()
        {
            rng.SetNextValue(1);
            rng.SetNextValue(1000);
            engine.MakeAttack(attacker, defender);
            Assert.IsTrue(defender.GetProperty("was_hit").ValueAsBool());
        }

        [Test]
        public void AttackerAccuracyIncreasesChanceToHit()
        {
            rng.SetNextValue(74);
            for (int i = 0; i < 4; i++)
            {
                attacker.Accuracy = (i * 25);
                var result = engine.MakeAttack(attacker, defender);
                if(i == 0)
                {
                    Assert.IsFalse(result.AttackHit);
                } else
                {
                    Assert.IsTrue(result.AttackHit);
                }
                rng.SetNextValue(74);
                rng.SetNextValue(1000);
            }
        }
    }
}