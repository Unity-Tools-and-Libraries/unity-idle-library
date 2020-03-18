using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

namespace Tests
{
    public class IdleEngineTest
    {
        private IdleEngine engine;

        [SetUp]
        public void setup()
        {
            var configuration = new GameConfigurationBuilder()
                .WithCustomGlobalProperty("multiplier", 1)
                .WithCustomEntityProperty("property")
                .WithEntity(new EntityDefinitionBuilder("food")
                    .WithCustomProperty("is-food")
                    .WithProduction("food", 1))
                .WithEntity(new EntityDefinitionBuilder("bar")
                    .WithConsumption("food", 3))
                .WithSingletonEntity(new SingletonEntityDefinitionBuilder("singleton"))
                .WithModifier(new ModifierDefinitionBuilder("effect").Active().Always().And().DoesNothing())
                .WithModifier(new ModifierDefinitionBuilder("food-bonus").Active().Always().And().HasEntityEffect(new GlobalEntityPropertyModifierEffectDefinition("outputs", "food", Literal.Of(1), EffectType.ADD)))
                .WithModifier(new ModifierDefinitionBuilder("food-penalty-1").Active().Always().And().HasEntityEffect(new EntityPropertyModifierEffectDefinition("bar", "inputs", "food", Literal.Of(1), EffectType.SUBTRACT)))
                .WithModifier(new ModifierDefinitionBuilder("food-penalty-2").Active().Always().And().HasEntityEffect(new EntityPropertyModifierEffectDefinition("bar", "inputs", "food", Literal.Of(1), EffectType.SUBTRACT)))
                .WithAchievement(new AchievementConfigurationBuilder("achievement").GainedWhen(new EntityPropertyMatcher("food", "enabled", Comparison.GREATER_THAN_OR_EQUAL, 1)))
                .Build();
            engine = new IdleEngine(configuration);
            engine.Update(1f);
        }

        [Test]
        public void EngineAddsEntitiesFromConfiguration()
        {
            Assert.AreEqual(2, engine.AllEntities.Count);
        }

        [Test]
        public void EngineAddsModifiersFromConfiguration()
        {
            Assert.AreEqual(4, engine.Modifiers.Count);
        }

        [Test]
        public void EngineCanApplyAnEffectThatAddsToAnEntitysOutput()
        {
            Assert.AreEqual(BigDouble.Floor(2), engine.AllEntities["food"].ProductionOutputs["food"].Value);
        }

        [Test]
        public void EngineCanRemoveAnEffectThatAddsToAnEntitysOutput()
        {
            Assert.AreEqual(BigDouble.Floor(2), engine.AllEntities["food"].ProductionOutputs["food"].Value);
        }

        [Test]
        public void EngineCanApplyAnEffectThatModifiedAnEntitiesInputs()
        {
            Assert.AreEqual(BigDouble.Floor(1), engine.AllEntities["bar"].ProductionInputs["food"].Value);
        }

        [Test]
        public void MultipleEffectsStack()
        {
            Assert.AreEqual(BigDouble.Floor(1), engine.AllEntities["bar"].ProductionInputs["food"].Value);
        }

        [Test]
        public void CanDefineACustomPropertyForAllEntities()
        {
            foreach (var entity in engine.AllEntities.Values)
            {
                Assert.IsTrue(entity.HasCustomProperty("property"), string.Format("failed for {0}", entity.EntityKey));
            }
        }

        [Test]
        public void CanDefineACustomPropertyForOneEntity()
        {
            foreach (var entity in engine.AllEntities.Values)
            {
                if (entity.EntityKey != "food")
                {
                    Assert.IsFalse(entity.HasCustomProperty("is-food"), string.Format("failed for {0}", entity.EntityKey));
                }
                else
                {
                    Assert.IsTrue(entity.HasCustomProperty("is-food"), string.Format("failed for {0}", entity.EntityKey));
                }

            }
        }

        [Test]
        public void EngineCanReturnSingletonTypes()
        {
            Assert.AreEqual(1, engine.AllSingletons.Count);
        }

        [Test]
        public void EngineReturnsZeroForNonExistantGlobalProperty() {
            Assert.AreEqual(BigDouble.Floor(0), engine.GetGlobalProperty("fake"));
        }

        [Test]
        public void EngineCanTriggerAchievements()
        {
            Assert.AreEqual(true, engine.GetAchievement("achievement").IsActive);
        }
    }

}