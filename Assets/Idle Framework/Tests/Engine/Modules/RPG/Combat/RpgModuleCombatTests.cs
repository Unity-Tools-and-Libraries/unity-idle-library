using NUnit.Framework;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using BreakInfinity;
using static io.github.thisisnozaku.idle.framework.Engine.IdleEngine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using System;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCombatTests : RequiresEngineTests
    {
        private Character attacker, defender;
        private RiggedRandom rng;

        [SetUp]
        public void Setup()
        {
            base.InitializeEngine();
            var module = new RpgModule();

            module.AddCreature(new CreatureDefinition.Builder().Build("1"));
            module.AddEncounter(new EncounterDefinition("1", Tuple.Create("1", 1)));

            engine.AddModule(module);
            engine.OverrideRandomNumberGenerator(rng);
            
            attacker = engine.CreateProperty("attacker", new Dictionary<string, ValueContainer>(), updater: "CharacterUpdateMethod").AsCharacter();
            attacker.Precision = 5;
            defender = engine.CreateProperty("encounter.creatures", new List<ValueContainer>() { engine.CreateValueContainer() }).ValueAsList()[0].AsCharacter();

            defender.MaximumHealth = defender.CurrentHealth = 10;
            attacker.Damage = 1;


            rng = new RiggedRandom();

            engine.GetProperty("action_phase").Set("combat");

            engine.OverrideRandomNumberGenerator(rng);

            engine.Start();
        }

        [Test]
        public void UpdateActionMeterOfEachCharacterInCombat()
        {
            rng.SetNextValue(0);
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(1), attacker.ActionMeter);
            Assert.AreEqual(new BigDouble(1), defender.ActionMeter);
        }

        [Test]
        public void WhenACharacterActionMeterEqualsActionSpeedTheyAct()
        {
            bool listenerCalled = false;
            engine.RegisterMethod("listener", (ie, args) => {
                listenerCalled = true;
                return null;
            });
            rng.SetNextValue(0);
            rng.SetNextValue(0);
            rng.SetNextValue(0); rng.SetNextValue(0); rng.SetNextValue(0); rng.SetNextValue(0); rng.SetNextValue(0); rng.SetNextValue(0); rng.SetNextValue(0); rng.SetNextValue(0);
            rng.SetNextValue(100);
            attacker.ActionMeter = 2;
            attacker.ActionMeter = 100000;
            attacker.Subscribe("test", CharacterActedEvent.EventName, "listener");
            engine.Update(.5f);
            Assert.IsTrue(listenerCalled);
        }

        [Test]
        public void WhenAnAttackHitsTheDefenderTakesDamageEqualToAttackerDamage()
        {
            
            rng.SetNextValue(1);
            rng.SetNextValue(1000);

            Assert.AreEqual(new BigDouble(10), defender.CurrentHealth);
            engine.MakeAttack(attacker, defender);
            Assert.AreEqual(new BigDouble(9), defender.CurrentHealth);

            rng.SetNextValue(1);
            rng.SetNextValue(1000);
            attacker.Damage = 5;
            engine.MakeAttack(attacker, defender);
            Assert.AreEqual(new BigDouble(4), defender.CurrentHealth);
        }

        [Test]
        public void WhenAnAttackHitsTheDefenderADamageTakenEventIsDispatched()
        {
            rng.SetNextValue(1);
            rng.SetNextValue(1000);
            int callCount = 0;
            UserMethod listener = (ie, args) => {
                callCount++;
                return null;
            };
            engine.RegisterMethod("listener", listener);
            ((ValueContainer)defender).Subscribe("test", DamageTakenEvent.EventName, "listener");
            ((ValueContainer)attacker).Subscribe("test", DamageInflictedEvent.EventName, "listener");
            engine.MakeAttack(attacker, defender);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void WhenRollIsGreaterThanAccuracyAttackMisses()
        {
            rng.SetNextValue(100);
            bool attackMissed = false;
            engine.RegisterMethod("handler", (e, a) =>
            {
                attackMissed = true;
                return null;
            });
            attacker.Subscribe("test", AttackMissEvent.EventName, "handler");
            engine.MakeAttack(attacker, defender);
            Assert.True(attackMissed);
        }

        [Test]
        public void WhenAttackHitsAndCritRollLessThanAttackerCritChance()
        {
            rng.SetNextValue(0);
            rng.SetNextValue(0);
            rng.SetNextValue(0);
            var result = engine.MakeAttack(attacker, defender);
            Assert.AreEqual("critical hit", result.Description);
        }

        public class RiggedRandom: Random
        {
            private Queue<int> nextValues = new Queue<int>();
            public void SetNextValue(int nextValue) => nextValues.Enqueue(nextValue);
            public override int Next(int maxValue)
            {
                if(nextValues.Count > 0)
                {
                    return nextValues.Dequeue();
                }
                throw new Exception();
            }
        }
    }
}