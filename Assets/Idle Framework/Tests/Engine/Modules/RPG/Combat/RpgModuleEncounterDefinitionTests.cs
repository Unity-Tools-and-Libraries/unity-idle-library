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
            engine.CreateProperty("configuration.base_creature_stats", new Dictionary<string, ValueContainer>()
            {
                { Character.Attributes.ACCURACY, engine.CreateValueContainer(0) },
                { Character.Attributes.CRITICAL_DAMAGE_MULTIPLIER, engine.CreateValueContainer(0) },
                { Character.Attributes.CRITICAL_HIT_CHANCE, engine.CreateValueContainer(0) },
                { Character.Attributes.DAMAGE, engine.CreateValueContainer(0) },
                { Character.Attributes.DEFENSE, engine.CreateValueContainer(0) },
                { Character.Attributes.EVASION, engine.CreateValueContainer(0) },
                { Character.Attributes.MAXIMUM_HEALTH, engine.CreateValueContainer(0) },
                { Character.Attributes.PENETRATION, engine.CreateValueContainer(0) },
                { Character.Attributes.PRECISION, engine.CreateValueContainer(0) },
                { Character.Attributes.RESILIENCE, engine.CreateValueContainer(0) }
            });
            var encounterDef = new EncounterDefinition("id", Tuple.Create("1", 0));
            engine.StartEncounter(encounterDef);
            Assert.NotNull(engine.GetProperty("encounter"));
            Assert.AreEqual("16", engine.GetProperty("encounter.creatures.0.id").ValueAsString());
        }
    }
}