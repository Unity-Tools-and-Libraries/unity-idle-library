using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCharacterTests : RpgModuleTestsBase
    {
        [Test]
        public void OnUpdateActionMeterIncreasesInCombat()
        {
            random.SetNextValues(0);

            Configure();
            engine.StartEncounter();
            //engine.SetActionPhase("combat");
            engine.GetPlayer<RpgPlayer>().Character.Update(engine, 1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgPlayer>().Character.ActionMeter);
        }

        [Test]
        public void OnUpdateActsWhenActionMeterFull()
        {
            Configure();

            random.SetNextValues(0, 0, 1, 1);

            engine.StartEncounter();
            engine.Watch(CharacterActedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer<RpgPlayer>().Character.Update(engine, (float)((BigDouble)engine.GetConfiguration("action_meter_required_to_act")).ToDouble());
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CharacterIsDeadWhenCurrentHealthIsZero()
        {
            Configure();

            var player = engine.GetPlayer<RpgPlayer>().Character;
            player.CurrentHealth = 0;
            Assert.IsFalse(player.IsAlive);
        }

        [Test]
        public void ApplyStatusAddsStatusOnCharacter()
        {
            rpgModule.AddStatus(new CharacterStatus.Builder().SetFlag("test").Build(engine, 1));

            Configure();
            engine.GetPlayer<RpgPlayer>().Character.Watch(StatusAddedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer<RpgPlayer>().Character.AddStatus(engine.GetStatuses()[1], new BigDouble(1));

            Assert.AreEqual(true, engine.GetPlayer<RpgPlayer>().Character.GetFlag("test"));
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void RemoveStatusUndoesStatusEffectOnCharacter()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.Watch(StatusRemovedEvent.EventName, "test", "globals.triggered = true");

            engine.GetPlayer<RpgPlayer>().Character.AddStatus(status, new BigDouble(1));
            engine.GetPlayer<RpgPlayer>().Character.RemoveStatus(status);

            Assert.IsFalse(engine.GetPlayer<RpgPlayer>().Character.GetFlag("test"));
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void UpdateChangesRemainingDurationOfAppliedStatuses()
        {
            random.SetNextValues(0);
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();
            engine.Start();

            engine.StartEncounter();
            engine.GetPlayer<RpgPlayer>().Character.AddStatus(status, new BigDouble(5));

            engine.Update(1);
            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgPlayer>().Character.Statuses[1].InitialTime);
            Assert.AreEqual(new BigDouble(4), engine.GetPlayer<RpgPlayer>().Character.Statuses[1].RemainingTime);
        }

        [Test]
        public void UpdateReducingTimeTo0RemoveStatus()
        {
            random.SetNextValues(0);
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();
            
            engine.Start();

            engine.StartEncounter();
            
            engine.GetPlayer<RpgPlayer>().Character.AddStatus(status, new BigDouble(1));
            engine.GetPlayer<RpgPlayer>().Character.Watch(StatusRemovedEvent.EventName, "test", "globals.triggered = true");
            engine.SetActionPhase("combat");

            engine.Update(1);
            Assert.AreEqual(0, engine.GetPlayer<RpgPlayer>().Character.Statuses.Count);
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void AddItemAddsItemToCharacterIfSlotAvailale()
        {

            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { }, null, null);
            engine.GetPlayer<RpgPlayer>().Character.Watch(ItemAddedEvent.EventName, "test", "globals.triggered = true");
            Assert.IsTrue(engine.GetPlayer<RpgPlayer>().Character.AddItem(item));
        }

        [Test]
        public void AddStatusNotDefinedInEngineThrows()
        {
            Configure();
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, engine.GetNextAvailableId());
            Assert.Throws<ArgumentNullException>(() =>
            {
                engine.GetPlayer<RpgPlayer>().Character.AddStatus(null, 1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                engine.GetPlayer<RpgPlayer>().Character.AddStatus(status, 1);
            });
        }

        [Test]
        public void AddItemToFullSlotFails()
        {

            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, null, null);
            Assert.IsTrue(engine.GetPlayer<RpgPlayer>().Character.AddItem(item));
            Assert.IsFalse(engine.GetPlayer<RpgPlayer>().Character.AddItem(item));
        }

        [Test]
        public void AddItemAppliesItsModifications()
        {
            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy.multiplier", Tuple.Create("value + 99", "value - 99") }
            }, null);

            var startingAccuracy = engine.GetPlayer<RpgPlayer>().Character.Accuracy.Total;
            engine.GetPlayer<RpgPlayer>().Character.AddItem(item);
            Assert.AreEqual(startingAccuracy * 100, engine.GetPlayer<RpgPlayer>().Character.Accuracy.Total);
            engine.GetPlayer<RpgPlayer>().Character.RemoveItem(item);
            Assert.AreEqual(startingAccuracy, engine.GetPlayer<RpgPlayer>().Character.Accuracy.Total);
        }

        [Test]
        public void AddItemEmitsEvent()
        {
            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy.multiplier", Tuple.Create("value * 100", "value / 100") }
            }, null);

            engine.GetPlayer<RpgPlayer>().Character.Watch(ItemAddedEvent.EventName, "test", "globals.triggered = true");

            engine.GetPlayer<RpgPlayer>().Character.AddItem(item);
            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void RemoveItemEmitsEvent()
        {
            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy.multiplier", Tuple.Create("value * 100", "value / 100") }
            }, null);

            engine.GetPlayer<RpgPlayer>().Character.Watch(ItemAddedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer<RpgPlayer>().Character.AddItem(item);            
            engine.GetPlayer<RpgPlayer>().Character.RemoveItem(item);
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void WhenPlayerDiesPlayerActionChangedToResurrecting()
        {
            random.SetNextValues(0);
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.Kill();
            Assert.AreEqual(RpgCharacter.Actions.REINCARNATING, engine.GetPlayer<RpgPlayer>().Character.Action);
        }

        [Test]
        public void WhenPlayerResurrectsStartEncounter()
        {
            random.SetNextValues(0, 0);
            Configure();

            var encounter = engine.StartEncounter();

            engine.Emit(CharacterResurrectedEvent.EventName, new CharacterResurrectedEvent(engine.GetPlayer<RpgPlayer>().Character));
            Assert.AreNotEqual(engine.GetCurrentEncounter(), encounter);
        }

        [Test]
        public void CharacterResetRemovesStatuses()
        {
            random.SetNextValues(0);
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.AddStatus(status, new BigDouble(1));
            engine.GetPlayer<RpgPlayer>().Character.MaximumHealth.BaseValue = 1;
            engine.GetPlayer<RpgPlayer>().Character.Kill();
            engine.GetPlayer<RpgPlayer>().Character.Reset();
            Assert.AreEqual(0, engine.GetPlayer<RpgPlayer>().Character.Statuses.Count);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgPlayer>().Character.CurrentHealth);
        }

        [Test]
        public void WhenCreatureDiesPlayerEarnsXpAndGold()
        {
            random.SetNextValues(0, 0);
            Configure();

            var encounter = engine.StartEncounter();

             encounter.Creatures[0].Kill();

            Assert.AreEqual(new BigDouble(10), engine.GetPlayer<RpgPlayer>().Character.GetResource("xp").Quantity);
            Assert.AreEqual(new BigDouble(10), engine.GetPlayer<RpgPlayer>().Character.GetResource("gold").Quantity);
        }

        [Test]
        public void CharactersCanAddAbilities()
        {
            Configure();

            var ability = new CharacterAbility.Builder().ChangeProperty("Accuracy.multiplier", "value * 2", "value / 2").Build( engine, engine.GetNextAvailableId());

            engine.GetPlayer<RpgPlayer>().Character.Watch(AbilityAddedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer<RpgPlayer>().Character.AddAbility(ability);

            Assert.AreEqual(new BigDouble(40), engine.GetPlayer<RpgPlayer>().Character.Accuracy.Total);
            Assert.IsTrue((bool?)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CharactersCanRemoveAbilities()
        {
            Configure();

            var ability = new CharacterAbility.Builder().ChangeProperty("Accuracy.multiplier", "value * 2", "value / 2").Build(engine, engine.GetNextAvailableId());

            engine.GetPlayer<RpgPlayer>().Character.AddAbility(ability);
            engine.GetPlayer<RpgPlayer>().Character.RemoveAbility(ability);

            Assert.AreEqual(new BigDouble(20), engine.GetPlayer<RpgPlayer>().Character.Accuracy.Total);
        }

        [Test]
        public void DamageReducesCurrentHealth()
        {
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.CurrentHealth = 10;

            engine.GetPlayer<RpgPlayer>().Character.InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgPlayer>().Character.CurrentHealth);
        }

        [Test]
        public void TakingDamageEmitsEvent()
        {
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.Watch(DamageTakenEvent.EventName, "test", "triggered = true");
            engine.GetPlayer<RpgPlayer>().Character.CurrentHealth = 10;

            engine.GetPlayer<RpgPlayer>().Character.InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgPlayer>().Character.CurrentHealth);
        }

        [Test]
        public void WhenDamageWouldReduceHealthToOrBelowZeroKillTheCharacter()
        {
            random.SetNextValues(0);
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.Watch(CharacterDiedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayer<RpgPlayer>().Character.CurrentHealth = 1;

            engine.GetPlayer<RpgPlayer>().Character.InflictDamage(1, null);
            

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CanConfigureCreatureXpValueCalculationScript()
        {
            random.SetNextValues(0);
            rpgModule.Creatures.XpValueCalculationScript = DynValue.NewString("return 1");

            Configure();

            var creature = new RpgCharacter(engine, 10);
            engine.Scripting.Evaluate(engine.GetConfiguration<CreaturesConfiguration>("creatures").Initializer, new Dictionary<string, object>() {
                {"creature", creature },
                { "level", BigDouble.One },
                { "definition", engine.GetCreatures()[1] }
                }, new List<string>() { "creature", "definition", "level" });
            Assert.AreEqual(BigDouble.One, creature.ExtraProperties["xp"]);
        }

        [Test]
        public void CalculatedCreatureHealth()
        {
            Configure();

            //engine.Logging.ConfigureLogging("creature.generate", UnityEngine.LogType.Log);

            var creature = new RpgCharacter(engine, 10);
            engine.Scripting.Evaluate(engine.GetConfiguration<CreaturesConfiguration>("creatures").Initializer, new Dictionary<string, object>() {
                { "creature", creature },
                { "level", BigDouble.One },
                { "definition", engine.GetCreatures()[1] }
                }, new List<string>() { "creature", "definition", "level" } );

            Assert.AreEqual(new BigDouble(10), creature.CurrentHealth);
            Assert.AreEqual(new BigDouble(10), creature.MaximumHealth.Total);
        }

        [Test]
        public void CalculatedPlayerHealth()
        {
            Configure();

            var creature = new RpgCharacter(engine, 10);
            engine.Scripting.Evaluate(engine.GetConfiguration<CreaturesConfiguration>("creatures").Initializer, new Dictionary<string, object>() {
                {"creature", creature },
                { "level", BigDouble.One },
                { "definition", engine.GetCreatures()[1] }
                }, new List<string>() { "creature", "definition", "level" });

            Assert.AreEqual(new BigDouble(20), engine.GetPlayer<RpgPlayer>().Character.CurrentHealth);
            Assert.AreEqual(new BigDouble(20), engine.GetPlayer<RpgPlayer>().Character.MaximumHealth.Total);
        }

        [Test]
        public void WhenGenerateCreatureReturnsCreatureWithAdditionalAttributes()
        {
            random.SetNextValues(0);

            Configure();

            Assert.AreEqual("bar", engine.GetCreatureDefinitions()[2].Properties["foo"]);
        }

        [Test]
        public void CreatureScaling()
        {
            Configure();
            var baseValue = new BigDouble(10);

            var result = engine.Scripting.EvaluateStringAsScript("return ScaleAttribute(value, level)",
                new Dictionary<string, object>()
                {
                    { "value", baseValue },
                    { "level", 1 }
                }).ToObject<BigDouble>();
            Assert.AreEqual(new BigDouble(10), result);
        }

        [Test]
        public void ErrorInPlayerGenerationScriptThrows()
        {
            rpgModule.Player.ValidationScript = DynValue.NewString("error('foobar')");

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                Configure();
            });
        }

        [Test]
        public void DefaultAttributeLevelIncrease()
        {
            Configure();
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<RpgPlayer>().Character.Accuracy.ChangePerLevel);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgPlayer>().Character.CriticalHitChance.ChangePerLevel);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgPlayer>().Character.CriticalHitDamageMultiplier.ChangePerLevel);
            Assert.AreEqual(new BigDouble(5), engine.GetPlayer<RpgPlayer>().Character.MaximumHealth.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<RpgPlayer>().Character.Penetration.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<RpgPlayer>().Character.Precision.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<RpgPlayer>().Character.Resilience.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<RpgPlayer>().Character.Defense.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayer<RpgPlayer>().Character.Evasion.ChangePerLevel);
        }

        [Test]
        public void IsAliveWhenCurrentHealthAbove0()
        {
            Configure();

            Assert.IsTrue(engine.GetPlayer<RpgPlayer>().Character.CurrentHealth > 0);
            Assert.IsTrue(engine.GetPlayer<RpgPlayer>().Character.IsAlive);

            engine.GetPlayer<RpgPlayer>().Character.CurrentHealth = 0;

            Assert.IsTrue(engine.GetPlayer<RpgPlayer>().Character.CurrentHealth == 0);
            Assert.IsFalse(engine.GetPlayer<RpgPlayer>().Character.IsAlive);
        }

        [Test]
        public void SetNumericAttributeOnDeserialization()
        {
            Configure();

            engine.GetPlayer<RpgPlayer>().Character.Accuracy.BaseValue = 500;
            var serialized = engine.GetSerializedSnapshotString();

            rpgModule = new RpgModule();
            engine = new framework.Engine.IdleEngine();
            Configure();

            engine.DeserializeSnapshotString(serialized);

            Assert.AreEqual(new BigDouble(500), engine.GetPlayer<RpgPlayer>().Character.Accuracy.BaseValue);
        }

        [Test]
        public void DeserializedEntityCanReplaceAnExisting()
        {
            Configure();

            var serialied = engine.GetSerializedSnapshotString();
            engine = new framework.Engine.IdleEngine();
            rpgModule = new RpgModule();

            Configure();
            engine.DeserializeSnapshotString(serialied);
        }
    }
}