using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ParentNotifyingMapTests : RequiresEngineTests
    {

        [Test]
        public void CanBeGeneratedFromExistingMap()
        {
            engine.Start();
            var existingMap = engine.SetProperty("path", new Dictionary<string, ValueContainer>() {
                { "stringKey", engine.CreateValueContainer("stringValue")}
            }).ValueAsMap();
            var container = engine.SetProperty("parent");
            var copy =  new ParentNotifyingDictionary(container, existingMap);
            Assert.AreEqual("stringValue", copy["stringKey"].ValueAsString());
            Assert.AreEqual(1, copy.Keys.Count);
            Assert.AreEqual(1, copy.Values.Count);
        }

        [Test]
        public void CanAddValue()
        {
            var map = engine.SetProperty("path", new Dictionary<string, ValueContainer>()).ValueAsMap();
            map.Add("key", engine.CreateValueContainer("value"));
        }

        [Test]
        public void AddKeyValuePair()
        {
            engine.Start();
            var map = engine.SetProperty("path", new Dictionary<string, ValueContainer>()).ValueAsMap();
            map.Add("key", engine.CreateValueContainer("string"));
            Assert.AreEqual("string", map["key"].ValueAsString());
        }

        [Test]
        public void ClearNullsValues()
        {
            var map = engine.SetProperty("path", new Dictionary<string, ValueContainer>()).ValueAsMap();
            map.Add("key", engine.CreateValueContainer("string"));
            map.Clear();
            Assert.AreEqual(null, map["key"].ValueAsRaw());
        }

        [Test]
        public void RemoveValueSetValueToNull()
        {
            var map = engine.SetProperty("path", new Dictionary<string, ValueContainer>()).ValueAsMap(); map.Add("key", engine.CreateValueContainer("string"));
            Assert.IsTrue(map.ContainsKey("key"));
            Assert.AreEqual("string", map["key"].ValueAsString());
            map.Remove("key");
            Assert.AreEqual(null, map["key"]);

        }
    }
}