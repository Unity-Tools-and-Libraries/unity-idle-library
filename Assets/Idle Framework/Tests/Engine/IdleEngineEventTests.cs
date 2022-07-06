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
            engine.RegisterMethod("listener", (ie, ev) =>
            {
                listenerCallCount++;
                return null;
            });
            engine.Subscribe("", "event", "listener()");
            engine.Start();
            engine.NotifyImmediately("event", null, engine);
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void BroadcastNotifiesListeners()
        {
            int listenerCallCount = 0;
            engine.RegisterMethod("listener", (ie, ev) =>
            {
                listenerCallCount++;
                return null;
            });
            engine.Subscribe("", "event", "listener()");
            engine.Start();
            engine.NotifyImmediately("event");
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void NotifyBubblesUp()
        {
            int listenerCallCount = 0;
            engine.RegisterMethod("listener", (ie, ev) =>
            {
                listenerCallCount++;
                return null;
            });
            var global = engine.CreateProperty("global");
            global.Subscribe("event", "event", "listener()");

            var middle = engine.CreateProperty("global.middle");
            middle.Subscribe("event", "event", "listener()");

            var bottom = engine.CreateProperty("global.middle.bottom");
            bottom.Subscribe("event", "event", "listener()");

            engine.Subscribe("", "event", "listener()");
            engine.Start();
            bottom.NotifyImmediately("event", "global");
            Assert.AreEqual(4, listenerCallCount);
        }

        [Test]
        public void CanUnsubscribeGlobalListeners()
        {
            int listenerCallCount = 0;
            engine.RegisterMethod("listener", (ie, ev) => {
                listenerCallCount++;
                return null;
            });
            var subscription = engine.Subscribe("", "event", "listener()");
            engine.Start();
            engine.NotifyImmediately("event");
            Assert.AreEqual(1, listenerCallCount);
            engine.Unsubscribe(subscription);
            engine.NotifyImmediately("event");
            Assert.AreEqual(1, listenerCallCount);
        }

        [Test]
        public void SubscribeToEngineReadyImmediatelyCalledIfEngineAlreadyStarted()
        {
            int listenerCallCount = 0;
            engine.RegisterMethod("listener", (ie, ev) => {
                listenerCallCount++;
                return null;
            });
            engine.Start();
            var subscription = engine.Subscribe("", EngineReadyEvent.EventName, "listener()");
            Assert.AreEqual(1, listenerCallCount);
            
        }
    }
}