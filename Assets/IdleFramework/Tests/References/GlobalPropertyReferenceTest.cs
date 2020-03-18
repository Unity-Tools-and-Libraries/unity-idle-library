using BreakInfinity;
using IdleFramework;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPropertyReferenceTest
{
    private IdleEngine engine;
    [SetUp]
    public void setup()
    {
        var config = new GameConfigurationBuilder().
            WithCustomGlobalProperty("exists", 2)
            .Build();
        engine = new IdleEngine(config);
    }
    [Test]
    public void Returns0ForANonexistantProperty()
    {
        Assert.AreEqual(BigDouble.Zero, new GlobalPropertyReference("foo").Get(engine));
    }
    [Test]
    public void ReturnsTheValueOfTheGlobalProperty()
    {
        Assert.AreEqual(BigDouble.Floor(2), new GlobalPropertyReference("exists").Get(engine));
    }
}
