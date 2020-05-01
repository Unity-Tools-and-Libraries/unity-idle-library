using BreakInfinity;
using IdleFramework;
using IdleFramework.Configuration;
using IdleFramework.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class IdleEngineTest
    {
        private IdleEngine engine;

        [SetUp]
        public void setup()
        {
            var configuration = new GameConfigurationBuilder()
                .WithCustomGlobalProperty("multiplier", Literal.Of(1))
                .WithEntity(new EntityDefinitionBuilder("food")
                    .WithCustomBooleanProperty("is-food", true)
                    .WithScaledOutput("food", 1)
                    .WithStartingQuantity(1))
                .WithEntity(new EntityDefinitionBuilder("bar")
                    .WithScaledInput("food", 3)
                    .WithStartingQuantity(1))
                .WithEntity(new EntityDefinitionBuilder("withVariants")
                    .WithFixedInput("food", 1)
                    .WithFixedOutput("food", 1)
                    .WithScaledInput("food", 1)
                    .WithScaledOutput("food", 1)
                    .WithVariant(new EntityDefinitionBuilder("variant-1")
                        .WithFixedInput("food", 2)
                        .WithFixedOutput("food", 2)
                        .WithScaledInput("food", 2)
                        .WithScaledOutput("food", 2)
                    )
                    )
                .WithAchievement(new AchievementConfigurationBuilder("achievement").GainedWhen(new EntityBooleanPropertyMatcher("food", "Enabled", true)))
                .Build();
            engine = new IdleEngine(configuration, null);
            engine.Update(1f);
        }

        [Test]
        public void EngineAddsEntitiesFromConfiguration()
        {
            Assert.AreEqual(3, engine.AllEntities.Count);
        }

        [Test]
        public void EngineThrowsExceptionWhenReferencingMissingGlobalNumberProperty() {
            Assert.Throws(typeof(MissingGlobalPropertyException), () =>
            {
                engine.GetGlobalNumberProperty("fake");
            });
        }

        [Test]
        public void EngineThrowsExceptionWhenReferencingMissingGlobalStringProperty()
        {
            Assert.Throws(typeof(MissingGlobalPropertyException), () =>
            {
                engine.GetGlobalStringProperty("fake");
            });
        }

        [Test]
        public void EngineThrowsExceptionWhenReferencingMissingGlobalBooleanProperty()
        {
            Assert.Throws(typeof(MissingGlobalPropertyException), () =>
            {
                engine.GetGlobalBooleanProperty("fake");
            });
        }

        [Test]
        public void EngineCanTriggerAchievements()
        {
            Assert.AreEqual(true, engine.GetAchievement("achievement").IsActive);
        }

        [Test]
        public void EngineSetupsEntityVariants()
        {
            Entity variant = engine.GetEntity("withVariants").GetVariant("variant-1");
            var expected = new Dictionary<string, BigDouble>() {
                { "food",           2 },
                { "bar",            0 },
                { "withVariants",   0 }
            };
            foreach(var expectedEntry in expected)
            {
                Assert.AreEqual(expectedEntry.Value, variant.ScaledInputs[expectedEntry.Key]);
                Assert.AreEqual(expectedEntry.Value, variant.ScaledOutputs[expectedEntry.Key]);
                Assert.AreEqual(expectedEntry.Value, variant.FixedInputs[expectedEntry.Key]);
                Assert.AreEqual(expectedEntry.Value, variant.FixedOutputs[expectedEntry.Key]);
            }
        }

    }

}