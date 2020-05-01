using IdleFramework;
using IdleFramework.Configuration;
using NUnit.Framework;

namespace Tests
{
    public class EntityStateMatcherTest
    {
        EntityStateMatcher matcher;
        Entity entity;
        IdleEngine engine;
        [SetUp]
        public void setup()
        {
            engine = new IdleEngine(new GameConfigurationBuilder()
                .WithEntity(new EntityDefinitionBuilder("food"))
                .WithEntity(new EntityDefinitionBuilder("test-1")
                    .WithScaledOutput("food", 0))
                .WithEntity(new EntityDefinitionBuilder("test-2")
                    .WithScaledOutput("food", 1))
                .WithEntity(new EntityDefinitionBuilder("test-3")
                    .WithStartingQuantity(1)
                    .WithScaledOutput("food", 2))
                .Build(), null);
            engine.Update(1f);
        }
        [Test]
        public void EntityNumberPropertyMatcherCanPerformEqualsMatch()
        {
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-1", "ScaledOutputs.food", Comparison.EQUALS, 1).Matches(engine));
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-2", "ScaledOutputs.food", Comparison.EQUALS, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-3", "ScaledOutputs.food", Comparison.EQUALS, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanPerformLessThanMatch()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-1", "ScaledOutputs.food", Comparison.LESS_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-2", "ScaledOutputs.food", Comparison.LESS_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-3", "ScaledOutputs.food", Comparison.LESS_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanPerformGreaterThanMatch()
        {
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-1", "ScaledOutputs.food", Comparison.GREATER_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-2", "ScaledOutputs.food", Comparison.GREATER_THAN, 1).Matches(engine));
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-3", "ScaledOutputs.food", Comparison.GREATER_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanMatchOnEntityProductionOutputs()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-1", "ScaledOutputs.food", Comparison.LESS_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanMatchOnEntityQuantity()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-3", "Quantity", Comparison.GREATER_THAN, 0).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanMatchOnEntityProductionInputs()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-1", "FixedInputs.food", Comparison.EQUALS, 0).Matches(engine));
        }

        public void EntityNumberPropertyMatcherReturnsFalseForNonexistantEntity()
        {
            Assert.IsFalse(new EntityNumberPropertyMatcher("missing", "whaterver", Comparison.EQUALS, 4414234).Matches(engine));
        }
    }
}
