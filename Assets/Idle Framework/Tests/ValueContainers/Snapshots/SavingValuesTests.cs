using io.github.thisisnozaku.idle.framework;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class SavingValuesTests : RequiresEngineTests
    {
        [Test]
        public void RestoringStringFromSnapshot()
        {
            engine.ConfigureLogging("engine.internal", null);
            var container = engine.CreateProperty("string", "string");
            var serialized = JsonConvert.SerializeObject(container.GetSnapshot());

            container.Set(null as string);

            var deserialized = JsonConvert.DeserializeObject<ValueContainer.Snapshot>(serialized);
            container.RestoreFromSnapshot(engine, deserialized);
            Assert.AreEqual("string", container.ValueAsString());
        }

        [Test]
        public void RestoringBoolFromSnapshot()
        {
            engine.ConfigureLogging("engine.internal", null);
            var container = engine.CreateProperty("bool", true);
            var serialized = JsonConvert.SerializeObject(container.GetSnapshot());

            container.Set(null as string);

            var deserialized = JsonConvert.DeserializeObject<ValueContainer.Snapshot>(serialized);
            container.RestoreFromSnapshot(engine, deserialized);
            Assert.IsTrue(container.ValueAsBool());
        }

        [Test]
        public void RestoringMapFromSnapshot()
        {
            engine.ConfigureLogging("engine.internal", null);
            var container = engine.CreateProperty("global", new Dictionary<string, ValueContainer>() {
                { "key", engine.CreateValueContainer("string") }
            });
            container.ValueAsMap()["nested"] = engine.CreateValueContainer(new Dictionary<string, ValueContainer>() {
                { "child", engine.CreateValueContainer("value") }
            });

            var serialized = JsonConvert.SerializeObject(container.GetSnapshot());

            container.Set(false);

            var deserialized = JsonConvert.DeserializeObject<ValueContainer.Snapshot>(serialized);
            container.RestoreFromSnapshot(engine, deserialized);
            Assert.AreEqual("string", container.ValueAsMap()["key"].ValueAsString());
            Assert.AreEqual("value", container.ValueAsMap()["nested"].ValueAsMap()["child"].ValueAsString());
        }

        [Test]
        public void EphemeralContainerNotSaved()
        {
            engine.ConfigureLogging("engine.internal", null);
            var container = engine.CreateProperty("global", new Dictionary<string, ValueContainer>() {
                { "key", engine.CreateValueContainer("string") }
            });
            container.ValueAsMap()["nested"] = engine.CreateValueContainer(new Dictionary<string, ValueContainer>() {
                { "child", engine.CreateValueContainer("value") }
            });
            container.ValueAsMap()["key"].SetEphemeral();

            var serialized = JsonConvert.SerializeObject(container.GetSnapshot());

            container.Set(false);

            var deserialized = JsonConvert.DeserializeObject<ValueContainer.Snapshot>(serialized);
            container.RestoreFromSnapshot(engine, deserialized);
            Assert.IsNull(container.ValueAsMap()["key"]);
            Assert.AreEqual("value", container.ValueAsMap()["nested"].ValueAsMap()["child"].ValueAsString());
        }
    }
}