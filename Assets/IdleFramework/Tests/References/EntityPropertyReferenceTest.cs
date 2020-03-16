using IdleFramework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    private IdleEngine engine;
    [SetUp]
    public void setup()
    {
        var config = new GameConfigurationBuilder()
            .WithEntity(new EntityDefinitionBuilder("exists").WithStartingQuantity(2))
            .Build();
        engine = new IdleEngine(config);
    }
    [Test]
    public void EntityMatcherReturnsFalseIfEntityDoesntExist()
    {
        Assert.IsFalse(new EntityPropertyMatcher("entity", "foo", Comparison.EQUALS, 1).Matches(engine));
    }

    [Test]
    public void EntityMatcherReturnsTrueIfPropertyLessThanMatcher()
    {
        Assert.IsTrue(new EntityPropertyMatcher("exists", "quantity", Comparison.LESS_THAN, 3).Matches(engine));
    }

    [Test]
    public void EntityMatcherReturnsTrueIfPropertyGreaterThanMatcher()
    {
        Assert.IsTrue(new EntityPropertyMatcher("exists", "quantity", Comparison.GREATER_THAN, 1).Matches(engine));
    }
}
