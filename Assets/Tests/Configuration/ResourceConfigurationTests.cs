using BreakInfinity;
using IdleFramework.Configuration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceConfigurationTests : RequiresEngineTests
{
    [SetUp]
    public void Setup()
    {
        InitializeEngine();
    }
    [Test]
    public void CanSpecifyBaseIncome()
    {
        var resourceConfig = new ResourceDefinitionBuilder("foo").
            WithBaseIncome(1).Build();
        var resource = resourceConfig.CreateValueReference(engine);
        engine.Update(1f);
        Assert.AreEqual(BigDouble.One, resource.ValueAsMap()["quantity"].ValueAsNumber());
        resourceConfig = new ResourceDefinitionBuilder("foo").
            WithBaseIncome(2).Build();
        resource = resourceConfig.CreateValueReference(engine);
        engine.Update(1f);
        Assert.AreEqual(new BigDouble(2), resource.ValueAsMap()["quantity"].ValueAsNumber());
    }
}
