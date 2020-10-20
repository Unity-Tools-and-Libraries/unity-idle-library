using IdleFramework.Configuration;
using NUnit.Framework;

namespace IdleFramework.Tests
{
    public class ValueReferenceEventSourceTest
    {
        private IdleEngine engine;
        private ValueReference valueReference;

        [SetUp]
        public void Setup()
        {
            engine = new IdleEngine(new EngineConfiguration() , null);
            valueReference = new ValueReferenceDefinitionBuilder().Build().CreateValueReference(engine);
        }
        [Test]
        public void CanSubscribeToEvent()
        {
            Assert.AreEqual(0, valueReference.EventListeners.Count);
            valueReference.Subscribe("customEvent", arg => { });
            Assert.AreEqual(1, valueReference.EventListeners.Count);
        }

        [Test]
        public void NotifiesValueChangeEventListenersWhenValueChanges()
        {
            bool listenerCalled = false;
            valueReference.Subscribe("valueChanged", newValue => {
                Assert.AreEqual(BreakInfinity.BigDouble.One, newValue);
                listenerCalled = true;
                });
            valueReference.Set(BreakInfinity.BigDouble.One);
            Assert.IsTrue(listenerCalled);
        }
    }
}