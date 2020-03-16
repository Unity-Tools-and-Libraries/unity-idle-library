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
                .WithModifier(new ModifierDefinitionBuilder("food-bonus").Active().Always().And().HasEntityEffect(new GlobalEntityPropertyModifierEffect("outputs", "food", 1, EffectType.ADD)))
                .WithModifier(new ModifierDefinitionBuilder("food-penalty-1").Active().Always().And().HasEntityEffect(new EntityPropertyModifierEffect("bar", "inputs", "food", 1, EffectType.SUBTRACT)))
                .WithModifier(new ModifierDefinitionBuilder("food-penalty-2").Active().Always().And().HasEntityEffect(new EntityPropertyModifierEffect("bar", "inputs", "food", 1, EffectType.SUBTRACT)))
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
    }

}