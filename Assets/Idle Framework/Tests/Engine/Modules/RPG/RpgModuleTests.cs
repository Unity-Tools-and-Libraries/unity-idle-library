using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleTests : RequiresEngineTests
    {

        [SetUp]
        public void Setup()
        {
            base.InitializeEngine();

            var module = new RpgModule();
            module.AddCreature(new CreatureDefinition.Builder().Build("1"));
            module.AddEncounter(new EncounterDefinition("1", Tuple.Create("1", 1)));

            engine.AddModule(module);
        }
        [Test]
        public void SetsAnActionPhaseStringProperty()
        {
            Assert.NotNull("", engine.GetProperty("action_phase"));
        }

        [Test]
        public void SetsPlayerProperty()
        {
            Assert.NotNull(engine.GetProperty("player"));
        }

        [Test]
        public void HasDefaultConfiguredItemSlots()
        {
            Assert.AreEqual(new Dictionary<string, int>()
            {
                {"head", 1 },
                {"neck", 1 },
                {"body", 1 },
                {"back", 1 },
                {"arms", 1 },
                {"hands", 1 },
                {"legs", 1 },
                {"feet", 1 },
                {"fingers", 1 },
            }, new RpgModule().ItemSlots);
        }
    }
}