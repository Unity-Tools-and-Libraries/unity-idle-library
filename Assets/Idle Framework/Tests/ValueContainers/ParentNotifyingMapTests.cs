using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

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
        var copy = new ParentNotifyingDictionary(existingMap);
        Assert.AreEqual("stringValue", copy["stringKey"].ValueAsString());
        Assert.AreEqual(1, copy.Keys.Count);
        Assert.AreEqual(1, copy.Values.Count);
    }

    [Test]
    public void CanAddValue()
    {
        var map = new ParentNotifyingDictionary();
        map.Add("key", "string");
    }

    [Test]
    public void AddNotifiesParent()
    {
        var map = new ParentNotifyingDictionary(new Dictionary<string, ValueContainer>() {
            { "stringKey", new ValueContainerDefinitionBuilder().WithStartingValue("stringValue").Build().CreateValueReference(engine)}
        });
        var container = new ValueContainer(null, map);
        
        bool listenerCalled = false;
        container.Subscribe(ValueContainer.Events.VALUE_CHANGED, nv => {
            listenerCalled = true;
        });
        map["one"] = new ValueContainer();
        Assert.IsTrue(listenerCalled);
    }

    [Test]
    public void AddKeyValuePair()
    {
        var map = new ParentNotifyingDictionary();
        map.Add(new KeyValuePair<string, ValueContainer>("key", "string"));
        Assert.AreEqual("string", map["key"].ValueAsString());
    }

    [Test]
    public void ClearNullsValues()
    {
        var map = new ParentNotifyingDictionary();
        map.Add(new KeyValuePair<string, ValueContainer>("key", "string"));
        map.Clear();
        Assert.AreEqual(ValueContainer.DEFAULT_VALUE, map["key"].ValueAsRaw());
    }

    [Test]
    public void RemoveValueSetValueToNull()
    {
        var map = new ParentNotifyingDictionary();
        map.Add("key", "string");
        Assert.IsTrue(map.ContainsKey("key"));
        Assert.AreEqual("string", map["key"].ValueAsString());
        map.Remove("key");
        Assert.AreEqual(ValueContainer.DEFAULT_VALUE, map["key"].ValueAsRaw());

    }
}
