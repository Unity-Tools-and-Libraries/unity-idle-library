using NUnit.Framework;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using BreakInfinity;
using static io.github.thisisnozaku.idle.framework.Engine.IdleEngine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;

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
            engine.AddModule(new RpgModule());
            engine.CreateProperty("configuration.action_meter_required_to_act", 2);
            engine.OverrideRandomNumberGenerator(rng);
            
            attacker = engine.CreateProperty("attacker", new Dictionary<string, ValueContainer>(), updater: "CharacterUpdateMethod").AsCharacter();
            defender = engine.CreateProperty("defender", new Dictionary<string, ValueContainer>(), updater: "CharacterUpdateMethod").AsCharacter();

            defender.MaximumHealth = defender.CurrentHealth = 10;
            attacker.Damage = 1;


            rng = new RiggedRandom();

            engine.GetProperty("action_phase").Set("combat");

            engine.OverrideRandomNumberGenerator(rng);

            engine.Start();
        }

        [Test]
        public void UpdateUpdatedActionMeterOfEachCharacterInCombat()
        {
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
            attacker.ActionMeter = 100000;
            attacker.Subscribe("test", CharacterActedEvent.EventName, "listener");
            engine.Update(2.5f);
            Assert.IsTrue(listenerCalled);
        }

        [Test]
        public void WhenAnAttackHitsTheDefenderTakesDamageEqualToAttackerDamage()
        {
            
            rng.SetNextValue(1);
            Assert.AreEqual(new BigDouble(10), defender.CurrentHealth);
            engine.MakeAttack(attacker, defender);
            Assert.AreEqual(new BigDouble(9), defender.CurrentHealth);
            attacker.Damage = 5;
            engine.MakeAttack(attacker, defender);
            Assert.AreEqual(new BigDouble(4), defender.CurrentHealth);
        }

        [Test]
        public void WhenAnAttackHitsTheDefenderADamageTakenEventIsDispatched()
        {
            rng.SetNextValue(1);
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

        public class RiggedRandom: System.Random
        {
            private int? nextValue;
            public void SetNextValue(int nextValue) => this.nextValue = nextValue;
            public override int Next(int maxValue)
            {
                if(nextValue.HasValue)
                {
                    return nextValue.Value;
                }
                throw new System.Exception();
            }
        }
    }
}