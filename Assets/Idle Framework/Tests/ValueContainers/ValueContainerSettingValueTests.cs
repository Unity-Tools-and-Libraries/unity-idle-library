using BreakInfinity;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class ValueContainerSettingValueTests : RequiresEngineTests
    {
        [Test]
        public void CanSetTheValueContainedByAReferenceToANumber()
        {
            var reference = engine.SetProperty("path", BigDouble.One);
            reference.Set(new BigDouble(2));
            Assert.AreEqual(reference.ValueAsNumber(), new BigDouble(2));
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToAString()
        {
            var reference = engine.SetProperty("path", "oldString");
            reference.Set("newString");
            Assert.AreEqual(reference.ValueAsString(), "newString");
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToAMap()
        {
            var initialDictionary = new Dictionary<string, ValueContainer>();
            var reference = engine.SetProperty("path", initialDictionary);
            var newDictionary = new Dictionary<string, ValueContainer>();
            reference.Set(newDictionary);
            Assert.AreEqual(reference.ValueAsMap(), newDictionary);
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToABool()
        {
            var reference = engine.SetProperty("path", true);
            reference.Set(false);
            Assert.IsFalse(reference.ValueAsBool());
        }
    }
}