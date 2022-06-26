using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System;
using static io.github.thisisnozaku.idle.framework.Engine.IdleEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class ValueContainerEventSourceTest : RequiresEngineTests
    {
        private ValueContainer valueReference;

        [SetUp]
        public void Setup()
        {
            valueReference = engine.CreateProperty("path", 0);
        }

        [Test]
        public void NotifiesValueChangeEventListenersWhenValueChanges()
        {
            bool listenerCalled = false;
            engine.RegisterMethod("changed", (ie, args) =>
            {
                var newValue = args[2];
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
            valueReference.Subscribe("", ValueChangedEvent.EventName, "changed");
            valueReference.Set(BreakInfinity.BigDouble.One);
            Assert.IsTrue(listenerCalled);
        }

        [Test]
        public void CanSubscribeByEventNameSubscriberDescriptionAndMethodReference()
        {
            bool listenerCalled = false;
            UserMethod changed = (ie, args) =>
            {
                var newValue = args[2];
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
            };
            engine.RegisterMethod(changed);
            engine.Start();
            valueReference.Subscribe("", ValueChangedEvent.EventName, changed);
            valueReference.Set(BreakInfinity.BigDouble.One);
            Assert.IsTrue(listenerCalled);
        }

        [Test]
        public void BroadcastCallsNotificationOnContainerAndAllChildren()
        {
            int listenerCallCount = 0;
            UserMethod changed = (ie, args) =>
            {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod(changed);
            engine.CreateProperty("one.two.three.four.five");
            engine.GetProperty("one").Subscribe("", "event", changed);
            engine.GetProperty("one.two").Subscribe("", "event", changed);
            engine.GetProperty("one.two.three").Subscribe("", "event", changed);
            engine.GetProperty("one.two.three.four").Subscribe("", "event", changed);
            engine.GetProperty("one.two.three.four.five").Subscribe("", "event", changed);
            engine.Start();
            engine.GetProperty("one.two.three").Broadcast("event");
            Assert.AreEqual(3, listenerCallCount);
        }
    }
}