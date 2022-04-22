using io.github.thisisnozaku.idle.framework.Configuration;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueContainerEventSourceTest
    {
        private IdleEngine engine;
        private ValueContainer valueReference;

        [SetUp]
        public void Setup()
        {
            engine = new IdleEngine(new EngineConfiguration() , null);
            valueReference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
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