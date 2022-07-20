using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
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
            engine.GetPlayer<RpgCharacter>().Update(engine, 1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgCharacter>().ActionMeter);
        }

        [Test]
        public void OnUpdateActsWhenActionMeterFull()
        {
            Configure();

            random.SetNextValues(0, 0, 1, 1);

            engine.SetActionPhase("combat");
            engine.StartEncounter();
            engine.Watch(CharacterActedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgCharacter>().Update(engine, (float)((BigDouble)engine.GetProperty("configuration.action_meter_required_to_act")).ToDouble());
            Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
        }

        [Test]
        public void CharacterIsDeadWhenCurrentHealthIsZero()
        {
            Configure();

            var player = engine.GetPlayer<RpgCharacter>();
            player.CurrentHealth = 0;
            Assert.IsFalse(player.IsAlive);
        }

        [Test]
        public void ApplyStatusAddsStatusOnCharacter()
        {
            rpgModule.AddStatus(new CharacterStatus.Builder().SetFlag("test").Build(engine, 1));

            Configure();
            engine.GetPlayer<RpgCharacter>().Watch(StatusAddedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgCharacter>().AddStatus(engine.GetStatuses()[1], new BigDouble(1));

            Assert.AreEqual(true, engine.GetPlayer<RpgCharacter>().GetFlag("test"));
            Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
        }

        [Test]
        public void RemoveStatusUndoesStatusEffectOnCharacter()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayer<RpgCharacter>().Watch(StatusRemovedEvent.EventName, "test", "triggered = true");

            engine.GetPlayer<RpgCharacter>().AddStatus(status, new BigDouble(1));
            engine.GetPlayer<RpgCharacter>().RemoveStatus(status);

            Assert.IsFalse(engine.GetPlayer<RpgCharacter>().GetFlag("test"));
            Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
        }

        [Test]
        public void UpdateChangesRemainingDurationOfAppliedStatuses()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();
            engine.Start();
            
            engine.GetPlayer<RpgCharacter>().AddStatus(status, new BigDouble(5));
            engine.GlobalProperties[RpgModule.Properties.ActionPhase] = "combat";

            engine.Update(1);
            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgCharacter>().Statuses[1].InitialTime);
            Assert.AreEqual(new BigDouble(4), engine.GetPlayer<RpgCharacter>().Statuses[1].RemainingTime);
        }

        [Test]
        public void UpdateReducingTimeTo0RemoveStatus()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();
            
            
            engine.Start();
            
            engine.GetPlayer<RpgCharacter>().AddStatus(status, new BigDouble(1));
            engine.GetPlayer<RpgCharacter>().Watch(StatusRemovedEvent.EventName, "test", "triggered = true");
            engine.GlobalProperties[RpgModule.Properties.ActionPhase] = "combat";

            engine.Update(1);
            Assert.AreEqual(0, engine.GetPlayer<RpgCharacter>().Statuses.Count);
            Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
        }

        [Test]
        public void AddItemAddsItemToCharacterIfSlotAvailale()
        {

            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, "", new string[] { }, null, null);
            engine.GetPlayer<RpgCharacter>().Watch(ItemAddedEvent.EventName, "test", "triggered = true");
            Assert.IsTrue(engine.GetPlayer<RpgCharacter>().AddItem(item));
        }

        [Test]
        public void AddStatusNotDefinedInEngineThrows()
        {
            Configure();
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, engine.GetNextAvailableId());
            Assert.Throws<ArgumentNullException>(() =>
            {
                engine.GetPlayer<RpgCharacter>().AddStatus(null, 1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                engine.GetPlayer<RpgCharacter>().AddStatus(status, 1);
            });
        }

        [Test]
        public void AddItemToFullSlotFails()
        {

            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, null, null);
            Assert.IsTrue(engine.GetPlayer<RpgCharacter>().AddItem(item));
            Assert.IsFalse(engine.GetPlayer<RpgCharacter>().AddItem(item));
        }

        [Test]
        public void AddItemAppliesItsModifications()
        {
            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy", Tuple.Create("value * 100", "value / 100") }
            }, null);

            var startingAccuracy = engine.GetPlayer<RpgCharacter>().Accuracy;
            engine.GetPlayer<RpgCharacter>().AddItem(item);
            Assert.AreEqual(startingAccuracy * 100, engine.GetPlayer<RpgCharacter>().Accuracy);
            engine.GetPlayer<RpgCharacter>().RemoveItem(item);
            Assert.AreEqual(startingAccuracy, engine.GetPlayer<RpgCharacter>().Accuracy);
        }

        [Test]
        public void AddItemEmitsEvent()
        {
            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy", Tuple.Create("value * 100", "value / 100") }
            }, null);

            engine.GetPlayer<RpgCharacter>().Watch(ItemAddedEvent.EventName, "test", "triggered = true");

            engine.GetPlayer<RpgCharacter>().AddItem(item);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void RemoveItemEmitsEvent()
        {
            Configure();

            var item = new RpgItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy", Tuple.Create("value * 100", "value / 100") }
            }, null);

            engine.GetPlayer<RpgCharacter>().Watch(ItemAddedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgCharacter>().AddItem(item);            
            engine.GetPlayer<RpgCharacter>().RemoveItem(item);
            Assert.IsTrue(engine.Scripting.Evaluate("return triggered").Boolean);
        }

        [Test]
        public void WhenPlayerDiesPlayerActionChangedToResurrecting()
        {
            Configure();

            engine.GetPlayer<RpgCharacter>().Kill();
            Assert.AreEqual(RpgCharacter.Actions.REINCARNATING, engine.GetPlayer<RpgCharacter>().Action);
        }

        [Test]
        public void CharacterResetRemovesStatuses()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayer<RpgCharacter>().AddStatus(status, new BigDouble(1));
            engine.GetPlayer<RpgCharacter>().MaximumHealth = 1;
            engine.GetPlayer<RpgCharacter>().Kill();
            engine.GetPlayer<RpgCharacter>().Reset();
            Assert.AreEqual(0, engine.GetPlayer<RpgCharacter>().Statuses.Count);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgCharacter>().CurrentHealth);
        }

        [Test]
        public void WhenCreatureDiesPlayerEarnsXpAndGold()
        {
            Configure();

            random.SetNextValues(0);

            var encounter = engine.StartEncounter();

            encounter.Creatures[0].Kill();

            Assert.AreEqual(new BigDouble(10), engine.GetPlayer<RpgCharacter>().Xp);
            Assert.AreEqual(new BigDouble(10), engine.GetPlayer<RpgCharacter>().Gold);
        }

        [Test]
        public void CharactersCanAddAbilities()
        {
            Configure();

            var ability = new CharacterAbility.Builder().ChangeProperty("Accuracy", "value * 2", "value / 2").Build( engine, engine.GetNextAvailableId());

            engine.GetPlayer<RpgCharacter>().AddAbility(ability);

            Assert.AreEqual(new BigDouble(20), engine.GetPlayer<RpgCharacter>().Accuracy);
        }

        [Test]
        public void CharactersCanRemoveAbilities()
        {
            Configure();

            var ability = new CharacterAbility.Builder().ChangeProperty("Accuracy", "value * 2", "value / 2").Build(engine, engine.GetNextAvailableId());

            engine.GetPlayer<RpgCharacter>().AddAbility(ability);
            engine.GetPlayer<RpgCharacter>().RemoveAbility(ability);

            Assert.AreEqual(new BigDouble(10), engine.GetPlayer<RpgCharacter>().Accuracy);
        }

        [Test]
        public void DamageReducesCurrentHealth()
        {
            Configure();

            engine.GetPlayer<RpgCharacter>().Watch(CharacterDiedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgCharacter>().CurrentHealth = 10;

            engine.GetPlayer<RpgCharacter>().InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgCharacter>().CurrentHealth);
        }

        [Test]
        public void TakingDamageEmitsEvent()
        {
            Configure();

            engine.GetPlayer<RpgCharacter>().Watch(CharacterDamagedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgCharacter>().CurrentHealth = 10;

            engine.GetPlayer<RpgCharacter>().InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgCharacter>().CurrentHealth);
        }

        [Test]
        public void WhenDamageWouldReduceHealthToOrBelowZeroKillTheCharacter()
        {
            Configure();

            engine.GetPlayer<RpgCharacter>().Watch(CharacterDiedEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgCharacter>().CurrentHealth = 1;

            engine.GetPlayer<RpgCharacter>().InflictDamage(1, null);
            

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }
    }
}