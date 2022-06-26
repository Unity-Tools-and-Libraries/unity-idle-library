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
            IdleEngine.UserMethod listener = (ie, ev) => {
                listenerCallCount++;
                return null; };
            engine.RegisterMethod(listener);
            engine.Subscribe("", "event", listener);
            engine.Start();
            engine.NotifyImmediately("event", args: null);
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void BroadcastNotifiesListeners()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, ev) => {
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
            IdleEngine.UserMethod listener = (ie, ev) => {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod("listener", listener);
            var global = engine.CreateProperty("global");
            global.Subscribe("event", "event", "listener");
            
            var middle = engine.CreateProperty("global.middle");
            middle.Subscribe("event", "event", "listener");

            var bottom = engine.CreateProperty("global.middle.bottom");
            bottom.Subscribe("event", "event", "listener");

            engine.Subscribe("", "event", "listener");
            engine.Start();
            bottom.NotifyImmediately("event", args: null);
            Assert.AreEqual(3, listenerCallCount);
        }

        [Test]
        public void CanUnsubscribeGlobalListeners()
        {
            int listenerCallCount = 0;
            IdleEngine.UserMethod listener = (ie, ev) => {
                listenerCallCount++;
                return null;
            };
            engine.RegisterMethod(listener);
            var subscription = engine.Subscribe("", "event", listener);
            engine.Start();
            engine.NotifyImmediately("event", args: null);
            Assert.AreEqual(1, listenerCallCount);
            engine.Unsubscribe(subscription);
            engine.NotifyImmediately("event", args: null);
            Assert.AreEqual(1, listenerCallCount);
        }
    }
}