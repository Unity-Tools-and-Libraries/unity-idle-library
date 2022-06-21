using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Definitions
{
    public class DefinitionTests : RequiresEngineTests
    {
        [Test]
        public void DefinitonCanBeTurnedIntoAValueContainer()
        {
            var def = new DummyDefinition("1", new Dictionary<string, object>()
            {
                {"foo", "string" },
                {"bar", 1 },
                {"baz", true },
                { "aMap", new Dictionary<string, object>()
                {
                    { "childString", "child" }
                } }
            });
            var instance = engine.InstantiateDefinitionInstance(def);
            engine.CreateProperty("path", instance);
            Assert.AreEqual("string", instance.ValueAsMap()["foo"].ValueAsString());
            Assert.AreEqual(BigDouble.One, instance.ValueAsMap()["bar"].ValueAsNumber());
            Assert.AreEqual(true, instance.ValueAsMap()["baz"].ValueAsBool());
            Assert.AreEqual("child", instance.ValueAsMap()["aMap"].ValueAsMap()["childString"].ValueAsString());
        }

        public class DummyDefinition : IDefinition
        {
            public string Id { get; }

            public IDictionary<string, object> Properties { get ; set; }

            public DummyDefinition(string id, IDictionary<string, object> properties)
            {
                Id = id;
                Properties = properties;
            }
        }
    }
}