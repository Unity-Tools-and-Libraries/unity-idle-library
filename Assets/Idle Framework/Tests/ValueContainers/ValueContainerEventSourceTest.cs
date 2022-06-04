using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System;
using static io.github.thisisnozaku.idle.framework.IdleEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class ValueContainerEventSourceTest : RequiresEngineTests
    {
        private ValueContainer valueReference;

        [SetUp]
        public void Setup()
        {
            valueReference = engine.SetProperty("path", 0);
        }

        [Test]
        public void NotifiesValueChangeEventListenersWhenValueChanges()
        {
            bool listenerCalled = false;
            engine.RegisterMethod("changed", (ie, c, ev) =>
            {
                var newValue = (ev[0] as ValueChangedEvent).NewValue;
                if (listenerCalled)
                {
                    Assert.AreEqual(BigDouble.One, newValue);
                }
                else
                {
                    Assert.AreEqual(BigDouble.Zero, newValue);
                }
                listenerCalled = true;
                return null;
            });
            engine.Start();
            valueReference.Subscribe("", ValueContainer.Events.VALUE_CHANGED, "changed");
            valueReference.Set(BreakInfinity.BigDouble.One);
            Assert.IsTrue(listenerCalled);
        }
    }
}