using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections.Generic;

public class ParentNotifyingMapTests : RequiresEngineTests
{
    
    [Test]
    public void CanBeGeneratedFromExistingMap()
    {
        var existingMap = new Dictionary<string, ValueContainer>() {
            { "stringKey", engine.CreateValueContainer("stringValue")}
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
        map.Add("key", engine.CreateValueContainer("value"));
    }

    [Test]
    public void AddNotifiesParent()
    {
        var map = new ParentNotifyingDictionary(new Dictionary<string, ValueContainer>() {
            { "stringKey", engine.CreateValueContainer("stringValue")}
        });
        var container = engine.CreateValueContainer(map);
        
        bool listenerCalled = false;
        container.Subscribe(ValueContainer.Events.VALUE_CHANGED, nv => {
            listenerCalled = true;
        });
        map["one"] = engine.CreateValueContainer();
        Assert.IsTrue(listenerCalled);
    }

    [Test]
    public void AddKeyValuePair()
    {
        var map = new ParentNotifyingDictionary();
        map.Add(new KeyValuePair<string, ValueContainer>("key", engine.CreateValueContainer("string")));
        Assert.AreEqual("string", map["key"].ValueAsString());
    }

    [Test]
    public void ClearNullsValues()
    {
        var map = new ParentNotifyingDictionary();
        map.Add(new KeyValuePair<string, ValueContainer>("key", engine.CreateValueContainer("string")));
        map.Clear();
        Assert.AreEqual(null, map["key"].ValueAsRaw());
    }

    [Test]
    public void RemoveValueSetValueToNull()
    {
        var map = new ParentNotifyingDictionary();
        map.Add("key", engine.CreateValueContainer("string"));
        Assert.IsTrue(map.ContainsKey("key"));
        Assert.AreEqual("string", map["key"].ValueAsString());
        map.Remove("key");
        Assert.AreEqual(null, map["key"].ValueAsRaw());

    }
}
