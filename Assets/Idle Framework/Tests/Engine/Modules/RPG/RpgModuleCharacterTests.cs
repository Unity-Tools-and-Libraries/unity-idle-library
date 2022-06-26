using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCharacterTests : RequiresEngineTests
    {
        private Character attacker, defender;
        [SetUp]
        public void Setup()
        {
            base.InitializeEngine();
            
            var module = new RpgModule();

            module.AddCreature(new CreatureDefinition.Builder().Build("1"));
            module.AddEncounter(new EncounterDefinition("1", Tuple.Create("1", 1)));

            engine.AddModule(module);
            engine.CreateProperty("configuration.action_meter_required_to_act", 2);

            attacker = engine.CreateProperty("attacker", new Dictionary<string, ValueContainer>(), updater: "CharacterUpdateMethod").AsCharacter();
            defender = engine.CreateProperty("defender", new Dictionary<string, ValueContainer>(), updater: "CharacterUpdateMethod").AsCharacter();

            defender.MaximumHealth = defender.CurrentHealth = 10;
            attacker.Damage = 1;

            engine.Start();
        }

        [Test]
        public void DoNotUpdateActionMeterWhenActionPhaseIsNotCombat()
        {
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(0), attacker.ActionMeter);
            Assert.AreEqual(new BigDouble(0), defender.ActionMeter);
        }

        [Test]
        public void HasItemSlots()
        {
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["head"]), attacker.ItemSlots["head"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["neck"]), attacker.ItemSlots["neck"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["body"]), attacker.ItemSlots["body"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["back"]), attacker.ItemSlots["back"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["arms"]), attacker.ItemSlots["arms"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["hands"]), attacker.ItemSlots["hands"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["legs"]), attacker.ItemSlots["legs"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["feet"]), attacker.ItemSlots["feet"]);
            Assert.AreEqual(new BigDouble(RpgModule.defaultItemSlots["fingers"]), attacker.ItemSlots["fingers"]);
        }

        [Test]
        public void CanAddAnItemToACharacter()
        {
            var item = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()
            {
                { ItemInstance.Attributes.UsedSlots, engine.CreateValueContainer(new List<ValueContainer>()
                {
                    engine.CreateValueContainer("head")
                }) }
            }).AsItem();
            attacker.AddItem(item);
            Assert.AreEqual(item, attacker.GetItems("head")[0]);
        }

        [Test]
        public void TryingToAddItemWithoutSpaceOnTargetFails()
        {
            var item = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()
            {
                { ItemInstance.Attributes.UsedSlots, engine.CreateValueContainer(new List<ValueContainer>()
                {
                    engine.CreateValueContainer("head")
                }) }
            }).AsItem();
            attacker.AddItem(item);
            Assert.False(attacker.AddItem(item));
            Assert.AreEqual(item, attacker.GetItems("head")[0]);
            Assert.AreEqual(1, attacker.GetItems("head").Count);
        }
    }
}