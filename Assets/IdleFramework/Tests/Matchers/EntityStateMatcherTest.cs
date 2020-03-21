using System;
using System.Collections;
using System.Collections.Generic;
using IdleFramework;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EntityStateMatcherTest
    {
        EntityStateMatcher matcher;
        GameEntity entity;
        IdleEngine engine;
        [SetUp]
        public void setup()
        {
            engine = new IdleEngine(new GameConfigurationBuilder()
                .WithEntity(new EntityDefinitionBuilder("food"))
                .WithEntity(new EntityDefinitionBuilder("test-1")
                    .WithOutput("food", 0))
                .WithEntity(new EntityDefinitionBuilder("test-2")
                    .WithOutput("food", 1))
                .WithEntity(new EntityDefinitionBuilder("test-3")
                    .WithStartingQuantity(1)
                    .WithOutput("food", 2))
                .Build());
            engine.Update(1f);
        }
        [Test]
        public void EntityNumberPropertyMatcherCanPerformEqualsMatch()
        {
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-1", "outputs", "food", Comparison.EQUALS, 1).Matches(engine));
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-2", "outputs", "food", Comparison.EQUALS, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-3", "outputs", "food", Comparison.EQUALS, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanPerformLessThanMatch()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-1", "Outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-2", "Outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-3", "Outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanPerformGreaterThanMatch()
        {
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-1", "Outputs", "food", Comparison.GREATER_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityNumberPropertyMatcher("test-2", "Outputs", "food", Comparison.GREATER_THAN, 1).Matches(engine));
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-3", "Outputs", "food", Comparison.GREATER_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanMatchOnEntityProductionOutputs()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-1", "outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanMatchOnEntityQuantity()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-3", "quantity", Comparison.GREATER_THAN, 0).Matches(engine));
        }

        [Test]
        public void EntityNumberPropertyMatcherCanMatchOnEntityProductionInputs()
        {
            Assert.IsTrue(new EntityNumberPropertyMatcher("test-1", "inputs", "food", Comparison.EQUALS, 0).Matches(engine));
        }

        public void EntityNumberPropertyMatcherReturnsFalseForNonexistantEntity()
        {
            Assert.IsFalse(new EntityNumberPropertyMatcher("missing", "whaterver", Comparison.EQUALS, 4414234).Matches(engine));
        }
    }
}
