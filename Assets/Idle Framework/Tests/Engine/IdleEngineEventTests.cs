using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class IdleEngineEventTests : RequiresEngineTests
    {
        [Test]
        public void NotifyTriggersListenersOnEngine()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, vc, ev) => {
                listenerCallCount++;
                return null; };
            engine.RegisterMethod(listener);
            engine.Subscribe("", "event", listener);
            engine.Start();
            engine.NotifyImmediately("event", null);
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void BroadcastNotifiesListeners()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, vc, ev) => {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod(listener);
            engine.Subscribe("", "event", listener);
            engine.Start();
            engine.Broadcast("event", null);
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void NotifyBubblesUp()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, vc, ev) => {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod(listener);
            var global = engine.SetProperty("global");
            global.Subscribe("event", "event", listener.Method.Name);
            
            var middle = engine.SetProperty("global.middle");
            middle.Subscribe("event", "event", listener.Method.Name);

            var bottom = engine.SetProperty("global.middle.bottom");
            bottom.Subscribe("event", "event", listener.Method.Name);

            engine.Subscribe("", "event", listener);
            engine.Start();
            bottom.NotifyImmediately("event", null);
            Assert.AreEqual(3, listenerCallCount);
        }

        [Test]
        public void CanUnsubscribeGlobalListeners()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, vc, ev) => {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod(listener);
            var subscription = engine.Subscribe("", "event", listener);
            engine.Start();
            engine.NotifyImmediately("event", null);
            Assert.AreEqual(1, listenerCallCount);
            engine.Unsubscribe(subscription);
            engine.NotifyImmediately("event", null);
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void CanUnsubscribeGlobalListenersByNameAndSource()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, vc, ev) => {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod(listener);
            var subscription = engine.Subscribe("", "event", listener);
            engine.Start();
            engine.NotifyImmediately("event", null);
            Assert.AreEqual(1, listenerCallCount);
            engine.Unsubscribe(subscription.SubscriberDescription, subscription.Event);
            engine.NotifyImmediately("event", null);
            Assert.AreEqual(1, listenerCallCount);
        }
    }
}