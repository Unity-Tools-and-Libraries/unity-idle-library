using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class SavingSnapshotTests : RequiresEngineTests
    {

        [Test]
        public void BoolContainerSavesToSnapshot()
        {
            var container = engine.CreateValueContainer(true);
            var snapshot = container.GetSnapshot();
            Assert.True((bool)snapshot.value);
            Assert.AreEqual(container.Id, snapshot.internalId);
        }

        [Test]
        public void SnapshotAndContainerHaveSameHashcode()
        {
            var container = engine.CreateValueContainer(true);
            var snapshot = container.GetSnapshot();
            Assert.AreEqual(container.GetHashCode(), snapshot.GetHashCode());
        }

        [Test]
        public void RestoringMapFromSnapshot()
        {
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>() {
                { "key", engine.CreateValueContainer("string") }
            }, null);
            container.ValueAsMap()["nested"] = engine.CreateValueContainer(new Dictionary<string, ValueContainer>() {
                { "child", engine.CreateValueContainer("value") }
            });

            var snapshot = container.GetSnapshot();

            container.Set(false);

            try
            {
                container.RestoreFromSnapshot(engine, snapshot);
            } catch (NullReferenceException e)
            {

            }
            Assert.AreEqual("string", container.ValueAsMap()["key"].ValueAsString());
            Assert.AreEqual("value", container.ValueAsMap()["nested"].ValueAsMap()["child"].ValueAsString());
        }
    }
}