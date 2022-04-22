using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class SavingSnapshotTests
    {
        private IdleEngine Engine;
        [SetUp]
        public void Setup()
        {
            Engine = new IdleEngine(null, null);
        }

        [Test]
        public void BoolContainerSavesToSnapshot()
        {
            var container = new ValueContainer(null, true, null);
            var snapshot = container.GetSnapshot();
            Assert.True((bool)snapshot.value);
            Assert.AreEqual(container.Id, snapshot.internalId);
        }

        [Test]
        public void SnapshotAndContainerHaveSameHashcode()
        {
            var container = new ValueContainer(null, true, null);
            Engine.RegisterReference(container);
            var snapshot = container.GetSnapshot();
            Assert.AreEqual(container.GetHashCode(), snapshot.GetHashCode());
        }

        [Test]
        public void RestoringMapFromSnapshot()
        {
            var container = new ValueContainer(null, new Dictionary<string, ValueContainer>() {
            { "key", "string" }
        }, null);
            container.ValueAsMap()["nested"] = new ValueContainer(container, new Dictionary<string, ValueContainer>() {
                { "child", "value" }
            });

            Engine.RegisterReference(container);
            var snapshot = container.GetSnapshot();
            var restored = new ValueContainer();
            restored.Id = container.Id;
            restored.RestoreFromSnapshot(Engine, snapshot);
            Assert.AreEqual("string", restored.ValueAsMap()["key"].ValueAsString());
            Assert.AreEqual("value", restored.ValueAsMap()["nested"].ValueAsMap()["child"].ValueAsString());
        }
    }
}