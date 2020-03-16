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
        var config = new GameConfigurationBuilder().Build();
        engine = new IdleEngine(config);
    }
    [Test]
    public void EntityMatcherReturnsFalseIfEntityDoesntExist()
    {
        Assert.IsFalse(new EntityPropertyMatcher("entity", "foo", Comparison.EQUALS, 1).Matches(engine));
    }
}
