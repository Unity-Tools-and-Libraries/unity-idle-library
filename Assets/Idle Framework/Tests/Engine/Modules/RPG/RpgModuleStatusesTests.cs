using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleStatusesTests : RequiresEngineTests
    {
        private Character character;
        [SetUp]
        public void setup()
        {
            base.InitializeEngine();
            var module = new RpgModule();
            module.AddStatus(new StatusDefinition.Builder()
                .SetsProperty("statused", "true")
                .Build("status", "status"));

            module.AddCreature(new CreatureDefinition.Builder()
                .WithHealthExpression("1")
                .Build("1"));

            module.AddEncounter(new EncounterDefinition("1", Tuple.Create("1", 1)));

            engine.AddModule(module);
            character = engine.CreateProperty("character").AsCharacter();
        }

        [Test]
        public void AddStatusMethodAppliesStatusToCharacter()
        {
            engine.ApplyStatus(character, "status", 1);
            Assert.True(engine.GetProperty("character.statused").ValueAsBool());
            Assert.AreEqual(new BigDouble(1), engine.GetProperty("character.statuses.status").ValueAsNumber());
        }

        [Test]
        public void RemoveStatusMethodRemovesStatusFromCharacter()
        {
            engine.ApplyStatus(character, "status", 1);
            engine.RemoveStatus(character, "status");
            Assert.False(engine.GetProperty("character.statused").ValueAsBool());
            Assert.Null(engine.GetProperty("character.statuses.status").ValueAsRaw());
        }

        [Test]
        public void WhenStatusDurationGoesTo0RemoveTheStatus()
        {
            engine.GetProperty("action_phase").Set("combat");
            engine.Start();
            engine.ApplyStatus(character, "status", 1);
            engine.GetProperty("character.statuses.status").Set(0);
            engine.Update(1);
            Assert.IsFalse(engine.GetProperty("character.statused").AsBool);
        }
    }
}