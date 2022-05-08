using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class IdleEngineEventSourceTest : RequiresEngineTests
    {
        
        [Test]
        public void CanSubscribeToEvent()
        {
            Assert.AreEqual(0, engine.EventListeners.Count);
            engine.Subscribe("customEvent", arg => { });
            Assert.AreEqual(1, engine.EventListeners.Count);
        }
    }
}