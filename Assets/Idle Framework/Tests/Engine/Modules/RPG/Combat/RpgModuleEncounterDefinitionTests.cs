using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.RpgModuleCombatTests;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleEncounterDefinitionTests : RequiresEngineTests
    {
        private RiggedRandom rng;

        [Test]
        public void CallingStartEncounterSetsEncounterProperties()
        {
            engine.CreateProperty("level", 0);
            var creatureDef = new CreatureDefinition.Builder()
                .WithHealthExpression("5")
                .WithDamageExpression("1")
                .WithIcon("")
                .Build("1");
            engine.SetDefinitions("creature", new Dictionary<string, IDefinition>()
            {
                {"1", creatureDef }
            });
            var encounterDef = new EncounterDefinition("id", Tuple.Create("1", 0));
            engine.StartEncounter(encounterDef);
            Assert.NotNull(engine.GetProperty("encounter"));
            Assert.AreEqual("4", engine.GetProperty("encounter.creatures.0.id").ValueAsString());
            Assert.AreEqual(new BigDouble(5), engine.GetProperty("encounter.creatures.0." + Character.Attributes.CURRENT_HEALTH).ValueAsNumber());
            Assert.AreEqual(new BigDouble(1), engine.GetProperty("encounter.creatures.0." + Character.Attributes.DAMAGE).ValueAsNumber());
        }
    }
}