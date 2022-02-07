using IdleFramework;
using IdleFramework.Configuration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentNotifyingMapTests
{
    private IdleEngine engine;
    [SetUp]
    public void Setup()
    {
        engine = new IdleEngine(null, null);
    }
    [Test]
    public void CanBeGeneratedFromExistingMap()
    {
        var existingMap = new Dictionary<string, ValueContainer>() {
            { "stringKey", new ValueContainerDefinitionBuilder().WithStartingValue("stringValue").Build().CreateValueReference(engine)}
        };
        var copy = new ParentNotifyingDictionary(null, existingMap);
        Assert.AreEqual("stringValue", copy["stringKey"].ValueAsString());
        Assert.AreEqual(1, copy.Keys.Count);
        Assert.AreEqual(1, copy.Values.Count);
    }
}
