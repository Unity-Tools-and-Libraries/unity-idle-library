using io.github.thisisnozaku.idle.framework.Configuration;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class IdleEngineEventSourceTest
    {
        private IdleEngine engine;

        [SetUp]
        public void Setup()
        {
            var configuration = new EngineConfiguration();
            engine = new IdleEngine(configuration, null);
        }

        [Test]
        public void CanSubscribeToEvent()
        {
            Assert.AreEqual(0, engine.EventListeners.Count);
            engine.Subscribe("customEvent", arg => { });
            Assert.AreEqual(1, engine.EventListeners.Count);
        }
    }
}