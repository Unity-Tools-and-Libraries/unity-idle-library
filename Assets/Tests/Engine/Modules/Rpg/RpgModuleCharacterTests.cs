using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCharacterTests : RpgModuleTestsBase
    {
        [Test]
        public void OnUpdateActionMeterIncreasesInCombat()
        {
            Configure();
            engine.SetActionPhase("combat");
            engine.GetPlayer().Update(engine, 1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer().ActionMeter);
        }

        [Test]
        public void OnUpdateActsWhenActionMeterFull()
        {
            Configure();

            random.SetNextValues(0, 0, 1, 1);

            engine.SetActionPhase("combat");
            engine.StartEncounter();
            engine.Watch(CharacterActedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer().Update(engine, (float)((BigDouble)engine.GetProperty("configuration.action_meter_required_to_act")).ToDouble());
            Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
        }

        [Test]
        public void CharacterIsDeadWhenCurrentHealthIsZero()
        {
            Configure();

            var player = engine.GetPlayer();
            player.CurrentHealth = 0;
            Assert.IsFalse(player.IsAlive);
        }

        [Test]
        public void ApplyStatusAddsStatusOnCharacter()
        {
            rpgModule.AddStatus(new CharacterStatus.Builder().SetFlag("test").Build(engine, 1));

            Configure();

            engine.GetPlayer().AddStatus(engine.GetStatuses()[1], new BigDouble(1));

            Assert.AreEqual(true, engine.GetPlayer().GetFlag("test"));
        }

        [Test]
        public void RemoveStatusUndoesStatusEffectOnCharacter()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayer().AddStatus(status, new BigDouble(1));
            engine.GetPlayer().RemoveStatus(status);

            Assert.IsFalse(engine.GetPlayer().GetFlag("test"));
        }

        [Test]
        public void UpdateChangesRemainingDurationOfAppliedStatuses()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();
            engine.Start();
            
            engine.GetPlayer().AddStatus(status, new BigDouble(5));
            engine.GlobalProperties[RpgModule.Properties.ActionPhase] = "combat";

            engine.Update(1);
            Assert.AreEqual(new BigDouble(5), engine.GetPlayer().Statuses[1].InitialTime);
            Assert.AreEqual(new BigDouble(4), engine.GetPlayer().Statuses[1].RemainingTime);
        }

        [Test]
        public void UpdateReducingTimeTo0RemoveStatus()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();
            
            
            engine.Start();
            
            engine.GetPlayer().AddStatus(status, new BigDouble(1));
            engine.GlobalProperties[RpgModule.Properties.ActionPhase] = "combat";

            engine.Update(1);
            Assert.AreEqual(0, engine.GetPlayer().Statuses.Count);
        }

        [Test]
        public void AddItemAddsItemToCharacterIfSlotAvailale()
        {

            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, new string[] { }, null);
            Assert.IsTrue(engine.GetPlayer().AddItem(item));
        }

        [Test]
        public void AddStatusNotDefinedInEngineThrows()
        {
            Configure();
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, engine.GetNextAvailableId());
            Assert.Throws<ArgumentNullException>(() =>
            {
                engine.GetPlayer().AddStatus(null, 1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                engine.GetPlayer().AddStatus(status, 1);
            });
        }

        [Test]
        public void AddItemToFullSlotFails()
        {

            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, new string[] { "head" }, null);
            Assert.IsTrue(engine.GetPlayer().AddItem(item));
            Assert.IsFalse(engine.GetPlayer().AddItem(item));
        }

        [Test]
        public void WhenPlayerDiesPlayerActionChangedToResurrecting()
        {
            Configure();

            engine.GetPlayer().Kill();
            Assert.AreEqual(RpgCharacter.Actions.REINCARNATING, engine.GetPlayer().Action);
        }

        [Test]
        public void CharacterResetRemovesStatuses()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayer().AddStatus(status, new BigDouble(1));
            engine.GetPlayer().MaximumHealth = 1;
            engine.GetPlayer().Kill();
            engine.GetPlayer().Reset();
            Assert.AreEqual(0, engine.GetPlayer().Statuses.Count);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer().CurrentHealth);
        }

        [Test]
        public void WhenCreatureDiesPlayerEarnsXpAndGold()
        {
            Configure();

            random.SetNextValues(0);

            var encounter = engine.StartEncounter();

            encounter.Creatures[0].Kill();

            Assert.AreEqual(new BigDouble(10), engine.GetPlayer().Xp);
            Assert.AreEqual(new BigDouble(10), engine.GetPlayer().Gold);
        }

        [Test]
        public void CharactersCanAddAbilities()
        {
            Configure();

            var ability = new AbilityDefinition(engine.GetNextAvailableId(), engine, "", new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy", Tuple.Create("value * 2", "value / 2") }
            });

            engine.GetPlayer().AddAbility(ability);

            Assert.AreEqual(new BigDouble(20), engine.GetPlayer().Accuracy);
        }

        [Test]
        public void CharactersCanRemoveAbilities()
        {
            Configure();

            var ability = new AbilityDefinition(engine.GetNextAvailableId(), engine, "", new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy", Tuple.Create("value * 2", "value / 2") }
            });

            engine.GetPlayer().AddAbility(ability);
            engine.GetPlayer().RemoveAbility(ability);

            Assert.AreEqual(new BigDouble(10), engine.GetPlayer().Accuracy);
        }

        [Test]
        public void DamageReducesCurrentHealth()
        {
            Configure();

            engine.GetPlayer().Watch(CharacterDiedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer().CurrentHealth = 10;

            engine.GetPlayer().InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayer().CurrentHealth);
            
        }

        [Test]
        public void WhenDamageWouldReduceHealthToOrBelowZeroKillTheCharacter()
        {
            Configure();

            engine.GetPlayer().Watch(CharacterDiedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer().CurrentHealth = 1;

            engine.GetPlayer().InflictDamage(1, null);
            

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }
    }
}