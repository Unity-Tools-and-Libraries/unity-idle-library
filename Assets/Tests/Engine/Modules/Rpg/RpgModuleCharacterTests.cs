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
            engine.GetPlayerCharacter<RpgCharacter>().Update(engine, 1);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayerCharacter<RpgCharacter>().ActionMeter);
        }

        [Test]
        public void OnUpdateActsWhenActionMeterFull()
        {
            Configure();

            random.SetNextValues(0, 0, 1, 1);

            engine.StartEncounter();
            engine.Watch(CharacterActedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayerCharacter<RpgCharacter>().Update(engine, (float)((BigDouble)engine.GetConfiguration("action_meter_required_to_act")).ToDouble());
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void CharacterIsDeadWhenCurrentHealthIsZero()
        {
            Configure();

            var player = engine.GetPlayerCharacter<RpgCharacter>();
            player.CurrentHealth = 0;
            Assert.IsFalse(player.IsAlive);
        }

        [Test]
        public void ApplyStatusAddsStatusOnCharacter()
        {
            rpgModule.AddStatus(new CharacterStatus.Builder().SetFlag("test").Build(engine, 1));

            Configure();
            engine.GetPlayerCharacter<RpgCharacter>().Watch(StatusAddedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayerCharacter<RpgCharacter>().AddStatus(engine.GetStatuses()[1], new BigDouble(1));

            Assert.AreEqual(true, engine.GetPlayerCharacter<RpgCharacter>().GetFlag("test"));
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void RemoveStatusUndoesStatusEffectOnCharacter()
        {
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().Watch(StatusRemovedEvent.EventName, "test", "globals.triggered = true");

            engine.GetPlayerCharacter<RpgCharacter>().AddStatus(status, new BigDouble(1));
            engine.GetPlayerCharacter<RpgCharacter>().RemoveStatus(status);

            Assert.IsFalse(engine.GetPlayerCharacter<RpgCharacter>().GetFlag("test"));
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
            engine.GetPlayerCharacter<RpgCharacter>().AddStatus(status, new BigDouble(5));

            engine.Update(1);
            Assert.AreEqual(new BigDouble(5), engine.GetPlayerCharacter<RpgCharacter>().Statuses[1].InitialTime);
            Assert.AreEqual(new BigDouble(4), engine.GetPlayerCharacter<RpgCharacter>().Statuses[1].RemainingTime);
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
            
            engine.GetPlayerCharacter<RpgCharacter>().AddStatus(status, new BigDouble(1));
            engine.GetPlayerCharacter<RpgCharacter>().Watch(StatusRemovedEvent.EventName, "test", "globals.triggered = true");
            engine.SetActionPhase("combat");

            engine.Update(1);
            Assert.AreEqual(0, engine.GetPlayerCharacter<RpgCharacter>().Statuses.Count);
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void AddItemAddsItemToCharacterIfSlotAvailale()
        {

            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { }, null, null);
            engine.GetPlayerCharacter<RpgCharacter>().Watch(ItemAddedEvent.EventName, "test", "globals.triggered = true");
            Assert.IsTrue(engine.GetPlayerCharacter<RpgCharacter>().AddItem(item));
        }

        [Test]
        public void AddStatusNotDefinedInEngineThrows()
        {
            Configure();
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, engine.GetNextAvailableId());
            Assert.Throws<ArgumentNullException>(() =>
            {
                engine.GetPlayerCharacter<RpgCharacter>().AddStatus(null, 1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                engine.GetPlayerCharacter<RpgCharacter>().AddStatus(status, 1);
            });
        }

        [Test]
        public void AddItemToFullSlotFails()
        {

            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, null, null);
            Assert.IsTrue(engine.GetPlayerCharacter<RpgCharacter>().AddItem(item));
            Assert.IsFalse(engine.GetPlayerCharacter<RpgCharacter>().AddItem(item));
        }

        [Test]
        public void AddItemAppliesItsModifications()
        {
            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy.multiplier", Tuple.Create("value + 99", "value - 99") }
            }, null);

            var startingAccuracy = engine.GetPlayerCharacter<RpgCharacter>().Accuracy.Total;
            engine.GetPlayerCharacter<RpgCharacter>().AddItem(item);
            Assert.AreEqual(startingAccuracy * 100, engine.GetPlayerCharacter<RpgCharacter>().Accuracy.Total);
            engine.GetPlayerCharacter<RpgCharacter>().RemoveItem(item);
            Assert.AreEqual(startingAccuracy, engine.GetPlayerCharacter<RpgCharacter>().Accuracy.Total);
        }

        [Test]
        public void AddItemEmitsEvent()
        {
            Configure();

            var item = new CharacterItem(engine.GetNextAvailableId(), engine, "", new string[] { "head" }, new Dictionary<string, Tuple<string, string>>()
            {
                { "Accuracy.multiplier", Tuple.Create("value * 100", "value / 100") }
            }, null);

            engine.GetPlayerCharacter<RpgCharacter>().Watch(ItemAddedEvent.EventName, "test", "globals.triggered = true");

            engine.GetPlayerCharacter<RpgCharacter>().AddItem(item);
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

            engine.GetPlayerCharacter<RpgCharacter>().Watch(ItemAddedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayerCharacter<RpgCharacter>().AddItem(item);            
            engine.GetPlayerCharacter<RpgCharacter>().RemoveItem(item);
            Assert.IsTrue(engine.Scripting.EvaluateStringAsScript("return globals.triggered").Boolean);
        }

        [Test]
        public void WhenPlayerDiesPlayerActionChangedToResurrecting()
        {
            random.SetNextValues(0);
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().Kill();
            Assert.AreEqual(RpgCharacter.Actions.REINCARNATING, engine.GetPlayerCharacter<RpgCharacter>().Action);
        }

        [Test]
        public void WhenPlayerResurrectsStartEncounter()
        {
            random.SetNextValues(0, 0);
            Configure();

            var encounter = engine.StartEncounter();

            engine.Emit(CharacterResurrectedEvent.EventName, new CharacterResurrectedEvent(engine.GetPlayerCharacter<RpgCharacter>()));
            Assert.AreNotEqual(engine.GetCurrentEncounter(), encounter);
        }

        [Test]
        public void CharacterResetRemovesStatuses()
        {
            random.SetNextValues(0);
            var status = new CharacterStatus.Builder().SetFlag("test", true).Build(engine, 1);
            rpgModule.AddStatus(status);
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().AddStatus(status, new BigDouble(1));
            engine.GetPlayerCharacter<RpgCharacter>().MaximumHealth.BaseValue = 1;
            engine.GetPlayerCharacter<RpgCharacter>().Kill();
            engine.GetPlayerCharacter<RpgCharacter>().Reset();
            Assert.AreEqual(0, engine.GetPlayerCharacter<RpgCharacter>().Statuses.Count);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth);
        }

        [Test]
        public void WhenCreatureDiesPlayerEarnsXpAndGold()
        {
            random.SetNextValues(0, 0);
            Configure();

            var encounter = engine.StartEncounter();

             encounter.Creatures[0].Kill();

            Assert.AreEqual(new BigDouble(10), engine.GetPlayerCharacter<RpgCharacter>().GetResource("xp").Quantity);
            Assert.AreEqual(new BigDouble(10), engine.GetPlayerCharacter<RpgCharacter>().GetResource("gold").Quantity);
        }

        [Test]
        public void CharactersCanAddAbilities()
        {
            Configure();

            var ability = new CharacterAbility.Builder().ChangeProperty("Accuracy.multiplier", "value * 2", "value / 2").Build( engine, engine.GetNextAvailableId());

            engine.GetPlayerCharacter<RpgCharacter>().Watch(AbilityAddedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayerCharacter<RpgCharacter>().AddAbility(ability);

            Assert.AreEqual(new BigDouble(40), engine.GetPlayerCharacter<RpgCharacter>().Accuracy.Total);
            Assert.IsTrue((bool?)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void CharactersCanRemoveAbilities()
        {
            Configure();

            var ability = new CharacterAbility.Builder().ChangeProperty("Accuracy.multiplier", "value * 2", "value / 2").Build(engine, engine.GetNextAvailableId());

            engine.GetPlayerCharacter<RpgCharacter>().AddAbility(ability);
            engine.GetPlayerCharacter<RpgCharacter>().RemoveAbility(ability);

            Assert.AreEqual(new BigDouble(20), engine.GetPlayerCharacter<RpgCharacter>().Accuracy.Total);
        }

        [Test]
        public void DamageReducesCurrentHealth()
        {
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth = 10;

            engine.GetPlayerCharacter<RpgCharacter>().InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth);
        }

        [Test]
        public void TakingDamageEmitsEvent()
        {
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().Watch(DamageTakenEvent.EventName, "test", "triggered = true");
            engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth = 10;

            engine.GetPlayerCharacter<RpgCharacter>().InflictDamage(5, null);

            Assert.AreEqual(new BigDouble(5), engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth);
        }

        [Test]
        public void WhenDamageWouldReduceHealthToOrBelowZeroKillTheCharacter()
        {
            random.SetNextValues(0);
            Configure();

            engine.GetPlayerCharacter<RpgCharacter>().Watch(CharacterDiedEvent.EventName, "test", "globals.triggered = true");
            engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth = 1;

            engine.GetPlayerCharacter<RpgCharacter>().InflictDamage(1, null);
            

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

            Assert.AreEqual(new BigDouble(20), engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth);
            Assert.AreEqual(new BigDouble(20), engine.GetPlayerCharacter<RpgCharacter>().MaximumHealth.Total);
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
            Assert.AreEqual(BigDouble.One, engine.GetPlayerCharacter<RpgCharacter>().Accuracy.ChangePerLevel);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayerCharacter<RpgCharacter>().CriticalHitChance.ChangePerLevel);
            Assert.AreEqual(new BigDouble(1), engine.GetPlayerCharacter<RpgCharacter>().CriticalHitDamageMultiplier.ChangePerLevel);
            Assert.AreEqual(new BigDouble(5), engine.GetPlayerCharacter<RpgCharacter>().MaximumHealth.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayerCharacter<RpgCharacter>().Penetration.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayerCharacter<RpgCharacter>().Precision.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayerCharacter<RpgCharacter>().Resilience.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayerCharacter<RpgCharacter>().Defense.ChangePerLevel);
            Assert.AreEqual(BigDouble.One, engine.GetPlayerCharacter<RpgCharacter>().Evasion.ChangePerLevel);
        }

        [Test]
        public void IsAliveWhenCurrentHealthAbove0()
        {
            Configure();

            Assert.IsTrue(engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth > 0);
            Assert.IsTrue(engine.GetPlayerCharacter<RpgCharacter>().IsAlive);

            engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth = 0;

            Assert.IsTrue(engine.GetPlayerCharacter<RpgCharacter>().CurrentHealth == 0);
            Assert.IsFalse(engine.GetPlayerCharacter<RpgCharacter>().IsAlive);
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