using BreakInfinity;
using NUnit.Framework;
using System;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueContainerEventSourceTest : RequiresEngineTests
    {
        private ValueContainer valueReference;

        [SetUp]
        public void Setup()
        {
            valueReference = engine.CreateValueContainer();
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
                Assert.AreEqual(listenerCalled ? BigDouble.One : BigDouble.Zero, newValue);
                listenerCalled = true;
                });
            valueReference.Set(BreakInfinity.BigDouble.One);
            Assert.IsTrue(listenerCalled);
        }

        [Test]
        public void CanUnsubscribeFromEvent()
        {
            Assert.AreEqual(0, valueReference.EventListeners.Count);
            Action<object> handler = arg => { };
            valueReference.Subscribe("customEvent", handler);
            valueReference.Unsubscribe("customEvent", handler);
            Assert.AreEqual(0, valueReference.EventListeners["customEvent"].Count);
        }
    }
}