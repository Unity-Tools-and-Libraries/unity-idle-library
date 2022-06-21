using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class ParentNotifyingListTest : RequiresEngineTests
    {

        [Test]
        public void CanBeGeneratedFromExistingList()
        {
            engine.Start();
            var existinglist = engine.CreateProperty("path", new List<ValueContainer>() {
                engine.CreateValueContainer("stringValue")
            }).ValueAsList();
            var container = engine.CreateProperty("parent");
            var copy = new ParentNotifyingList(container, existinglist);
            Assert.AreEqual("stringValue", copy[0].ValueAsString());
            Assert.AreEqual(1, copy.Count);
        }

        [Test]
        public void CanAddValue()
        {
            var list = engine.CreateProperty("path", new List<ValueContainer>()).ValueAsList();
            list.Add(engine.CreateValueContainer("value"));
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanSetValue()
        {
            var list = engine.CreateProperty("path", new List<ValueContainer>()).ValueAsList();
            list.Add(engine.CreateValueContainer("value"));
            list[0] = engine.CreateValueContainer("value2");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("value2", list[0].ValueAsString());
        }

        [Test]
        public void ClearNullsValues()
        {
            var list = engine.CreateProperty("path", new List<ValueContainer>()).ValueAsList();
            list.Add(engine.CreateValueContainer("string"));
            list.Clear();
            Assert.AreEqual(null, list[0].ValueAsRaw());
        }

        [Test]
        public void RemoveValueSetValueToNull()
        {
            var list = engine.CreateProperty("path", new List<ValueContainer>()).ValueAsList(); list.Add(engine.CreateValueContainer("string"));
            Assert.AreEqual("string", list[0].ValueAsString());
            list.Remove(list[0]);
            Assert.AreEqual(0, list.Count);

        }
    }
}