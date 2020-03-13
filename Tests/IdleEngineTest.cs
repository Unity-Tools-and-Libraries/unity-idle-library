using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

namespace Test {
    public class IdleEngineTest
    {
        private IdleEngine engine;
        
        [SetUp]
        public void setup()
        {
            var configuration = new GameConfigurationBuilder()
                .WithEntity(new EntityDefinitionBuilder("food")
                    .WithProduction("food", 1))
                .WithEntity(new EntityDefinitionBuilder("bar")
                    .WithConsumption("food", 3))
                .WithModifier(new ModifierDefinitionBuilder("effect").Active().Always().And().DoesNothing())
                .WithModifier(new ModifierDefinitionBuilder("food-bonus").Active().Always().And().HasEntityEffect(new GlobalEntityPropertyModifierEffect("outputs", "food", 1, EffectType.ADD)))
                .WithModifier(new ModifierDefinitionBuilder("food-penalty-1").Active().Always().And().HasEntityEffect(new EntityPropertyModifierEffect("bar", "inputs", "food", 1, EffectType.SUBTRACT)))
                .WithModifier(new ModifierDefinitionBuilder("food-penalty-2").Active().Always().And().HasEntityEffect(new EntityPropertyModifierEffect("bar", "inputs", "food", 1, EffectType.SUBTRACT)))
                .Build();
            engine = new IdleEngine(configuration);
            engine.Update();
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
            Assert.AreEqual(BigDouble.Floor(2), engine.AllEntities["food"].ProductionOutputs["food"]);
        }

        [Test]
        public void EngineCanApplyAnEffectThatModifiedAnEntitiesInputs()
        {
            Assert.AreEqual(BigDouble.Floor(1), engine.AllEntities["bar"].ProductionInputs["food"]);
        }

        [Test]
        public void MultipleEffectsStack()
        {
            Assert.AreEqual(BigDouble.Floor(1), engine.AllEntities["bar"].ProductionInputs["food"]);
        }
    }

}