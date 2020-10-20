using IdleFramework.Configuration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Tests
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