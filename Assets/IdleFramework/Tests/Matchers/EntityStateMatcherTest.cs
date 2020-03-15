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
                    .WithProduction("food", 0))
                .WithEntity(new EntityDefinitionBuilder("test-2")
                    .WithProduction("food", 1))
                .WithEntity(new EntityDefinitionBuilder("test-3")
                    .WithStartingQuantity(1)
                    .WithProduction("food", 2))
                .Build());
            engine.Update(1f);
        }
        [Test]
        public void EntityPropertyMatcherCanPerformEqualsMatch()
        {
            Assert.IsFalse(new EntityPropertyMatcher("test-1", "Outputs", "food", Comparison.EQUALS, 1).Matches(engine));
            Assert.IsTrue(new EntityPropertyMatcher("test-2", "Outputs", "food", Comparison.EQUALS, 1).Matches(engine));
            Assert.IsFalse(new EntityPropertyMatcher("test-3", "Outputs", "food", Comparison.EQUALS, 1).Matches(engine));
        }

        [Test]
        public void EntityPropertyMatcherCanPerformLessThanMatch()
        {
            Assert.IsTrue(new EntityPropertyMatcher("test-1", "Outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityPropertyMatcher("test-2", "Outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityPropertyMatcher("test-3", "Outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityPropertyMatcherCanPerformGreaterThanMatch()
        {
            Assert.IsFalse(new EntityPropertyMatcher("test-1", "Outputs", "food", Comparison.GREATER_THAN, 1).Matches(engine));
            Assert.IsFalse(new EntityPropertyMatcher("test-2", "Outputs", "food", Comparison.GREATER_THAN, 1).Matches(engine));
            Assert.IsTrue(new EntityPropertyMatcher("test-3", "Outputs", "food", Comparison.GREATER_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityPropertyMatcherCanMatchOnEntityProductionOutputs()
        {
            Assert.IsTrue(new EntityPropertyMatcher("test-1", "outputs", "food", Comparison.LESS_THAN, 1).Matches(engine));
        }

        [Test]
        public void EntityPropertyMatcherCanMatchOnEntityQuantity()
        {
            Assert.IsTrue(new EntityPropertyMatcher("test-3", "quantity", "food", Comparison.GREATER_THAN, 0).Matches(engine));
        }

        [Test]
        public void EntityPropertyMatcherCanMatchOnEntityProductionInputs()
        {
            Assert.IsTrue(new EntityPropertyMatcher("test-1", "inputs", "food", Comparison.EQUALS, 0).Matches(engine));
        }

        [Test]
        public void EntityPropertyMatcherThrowsExceptionOnBadProperty()
        {
            Assert.That(() =>
            {
                new EntityPropertyMatcher("test-1", "fdfs", "food", Comparison.GREATER_THAN, 1);
            }, Throws.ArgumentException);
            
        }
    }
}
