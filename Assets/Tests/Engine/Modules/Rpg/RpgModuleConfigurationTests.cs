using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleConfigurationTests : RpgModuleTestsBase
    {
        
        [Test]
        public void ConfiguresProperties()
        {
            Configure();

            Assert.AreEqual(typeof(RpgCharacter), engine.GlobalProperties["player"].GetType()); // Sets a player
            Assert.AreEqual("", engine.GlobalProperties["actionPhase"] as string); // Set an action phase
            Assert.NotNull(engine.GetProperty("definitions.encounters"));
            Assert.NotNull(engine.GetConfiguration("characterItemSlots"));
            Assert.AreEqual(new BigDouble(1), engine.GetProperty("stage"));
            Assert.AreEqual(new BigDouble(2), engine.GetConfiguration()["action_meter_required_to_act"]);
            Assert.AreEqual(RpgModule.DEFAULT_XP_CALCULATION_METHOD, engine.GetConfiguration()["xp_calculation_method"]);
            Assert.AreEqual(RpgModule.DEFAULT_gold_calculation_script, engine.GetConfiguration()["gold_calculation_script"]);
            Assert.NotNull(engine.GetProperty<RpgCharacter>("player"));
            Assert.NotNull(engine.GetDefinitions()["statuses"]);
        }

        [Test]
        public void ModuleFailesIfNoEncountersDefined()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.AddModule(new RpgModule());
            });
        }

        [Test]
        public void DefaultXpBasedOnLevel()
        {
            var result = engine.Scripting.Evaluate("return 10 * math.pow(2, level - 1)", new Dictionary<string, object>()
            {
                { "level", 1 }
            });
            Assert.AreEqual(new BigDouble(10), result.ToObject<BigDouble>());
        }

        [Test]
        public void CannotAddEncounterWithNotSpecifiedCreature()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                new RpgModule().AddEncounter(new EncounterDefinition(1, Tuple.Create(1L, 0)));
            });
        }

        [Test]
        public void CannotAddEncounterWithNoOptions()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                new RpgModule().AddEncounter(new EncounterDefinition(1));
            });
        }
    }
}