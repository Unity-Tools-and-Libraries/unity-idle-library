using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class IdleEngineTest
    {
        private IdleEngine engine;

        [SetUp]
        public void setup()
        {
            engine = new IdleEngine(null);
            engine.SetProperty("globalBoolean", true);
            engine.SetProperty("globalBooleanNoStartingValue");

            engine.SetProperty("globalNumber", BigDouble.One);
            engine.SetProperty("globalNumberNoStartingValue");

            engine.SetProperty("globalString", "startingValue");
            engine.SetProperty("globalStringNoStartingValue");

            engine.SetProperty("globalMapNoStartingValue");
            engine.SetProperty("globalMap", new Dictionary<string, ValueContainer>()
            {
                { "foo", engine.CreateValueContainer("bar") }
            });
            engine.RegisterMethod("update", (engine, vc, ev) =>
            {
                return (BigDouble)ev[1] + BigDouble.One;
            });
            engine.SetProperty("incrementingNumberValue", 0, updater: "update");
        }

        [Test]
        public void TryingToGetDeclaredGlobalPropertyReturnsReferenceToThatProperty()
        {
            var propertyReference = engine.GetProperty("globalBoolean");
            Assert.NotNull(propertyReference);
        }

        // Global properties
        [Test]
        public void BooleanGlobalPropertyWithDefaultReturnsThatDefault()
        {
            ValueContainer propertyReference = engine.GetProperty("globalBoolean");
            Assert.IsTrue(propertyReference.ValueAsBool());
        }

        [Test]
        public void BooleanGlobalPropertyWithNoDefaultStartsFalse()
        {
            ValueContainer propertyReference = engine.GetProperty("globalBooleanNoStartingValue");
            Assert.IsFalse(propertyReference.ValueAsBool());
        }

        [Test]
        public void NumberGlobalPropertyWithDefaultReturnsThatDefault()
        {
            ValueContainer propertyReference = engine.GetProperty("globalNumber");
            Assert.AreEqual(BigDouble.One, propertyReference.ValueAsNumber());
        }

        [Test]
        public void NumberGlobalPropertyWithNoDefaultReturnsZero()
        {
            ValueContainer propertyReference = engine.GetProperty("globalNumberNoStartingValue");
            Assert.AreEqual(BigDouble.Zero, propertyReference.ValueAsNumber());
        }

        [Test]
        public void StringGlobalPropertyWithNoDefaultReturnsZeroString()
        {
            ValueContainer propertyReference = engine.GetProperty("globalStringNoStartingValue");
            Assert.AreEqual("", propertyReference.ValueAsString());
        }

        [Test]
        public void StringGlobalPropertyWithNoDefaultReturnsDefault()
        {
            ValueContainer propertyReference = engine.GetProperty("globalString");
            Assert.AreEqual("startingValue", propertyReference.ValueAsString());
        }

        [Test]
        public void MapGlobalPropertyWithNoDefaultReturnsEmpty()
        {
            ValueContainer propertyReference = engine.GetProperty("globalMapNoStartingValue");
            Assert.AreEqual(null, propertyReference.ValueAsMap());
        }

        [Test]
        public void CallingUpdateOnEngineCallsUpdateOnAllValues()
        {
            ValueContainer propertyReference = engine.GetProperty("incrementingNumberValue");
            int listenerCalled = 0;
            engine.RegisterMethod("method", (ie, c, ev) =>
            {
                listenerCalled++;
                return null;
            });
            engine.Start();
            propertyReference.Subscribe("", ValueChangedEvent.EventName, "method");
            engine.Update(1f);
            Assert.AreEqual(2, listenerCalled);
        }

        [Test]
        public void CanSubscribeToEvents()
        {
            engine.RegisterMethod("method", (ie, c, ev) => null);
            engine.Subscribe("", "customEvent", "method");
        }

        [Test]
        public void SettingGlobalPropertySetsPathOfResultingContainer()
        {
            var container = engine.SetProperty("property");
            Assert.AreEqual("property", container.Path);
        }

        [Test]
        public void EmptyPathFailsValidation()
        {

        }
    }
}