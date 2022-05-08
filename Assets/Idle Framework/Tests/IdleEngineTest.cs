using BreakInfinity;
using NUnit.Framework;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class IdleEngineTest
    {
        private IdleEngine engine;

        [SetUp]
        public void setup()
        {
            engine = new IdleEngine( null);
            engine.SetGlobalProperty("globalBoolean", true);
            engine.SetGlobalProperty("globalBooleanNoStartingValue");

            engine.SetGlobalProperty("globalNumber", BigDouble.One);
            engine.SetGlobalProperty("globalNumberNoStartingValue");

            engine.SetGlobalProperty("globalString", "startingValue");
            engine.SetGlobalProperty("globalStringNoStartingValue");

            engine.SetGlobalProperty("globalMapNoStartingValue");
            engine.SetGlobalProperty("globalMap", new Dictionary<string, ValueContainer>()
            {
                { "foo", engine.CreateValueContainer("bar") }
            });

            engine.GetGlobalProperty("incrementingNumberValue").SetUpdater((engine, deltaTime, currentValue, modifiers) =>
                {
                    return (BigDouble)currentValue + BigDouble.One;
                });

            
        }

        [Test]
        public void TryingToGetDeclaredGlobalPropertyReturnsReferenceToThatProperty()
        {
            var propertyReference = engine.GetGlobalProperty("globalBoolean");
            Assert.NotNull(propertyReference);
        }

        // Global properties
        [Test]
        public void BooleanGlobalPropertyWithDefaultReturnsThatDefault()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalBoolean");
            Assert.IsTrue(propertyReference.ValueAsBool());
        }

        [Test]
        public void BooleanGlobalPropertyWithNoDefaultStartsFalse()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalBooleanNoStartingValue");
            Assert.IsFalse(propertyReference.ValueAsBool());
        }

        [Test]
        public void NumberGlobalPropertyWithDefaultReturnsThatDefault()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalNumber");
            Assert.AreEqual(BigDouble.One, propertyReference.ValueAsNumber());
        }

        [Test]
        public void NumberGlobalPropertyWithNoDefaultReturnsZero()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalNumberNoStartingValue");
            Assert.AreEqual(BigDouble.Zero, propertyReference.ValueAsNumber());
        }

        [Test]
        public void StringGlobalPropertyWithNoDefaultReturnsZeroString()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalStringNoStartingValue");
            Assert.AreEqual("0", propertyReference.ValueAsString());
        }

        [Test]
        public void StringGlobalPropertyWithNoDefaultReturnsDefault()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalString");
            Assert.AreEqual("startingValue", propertyReference.ValueAsString());
        }

        [Test]
        public void MapGlobalPropertyWithNoDefaultReturnsEmpty()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalMapNoStartingValue");
            Assert.AreEqual(null, propertyReference.ValueAsMap());
        }

        [Test]
        public void CallingUpdateOnEngineCallsUpdateOnAllValues()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("incrementingNumberValue");
            int listenerCalled = 0;
            propertyReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, v =>
            {
                listenerCalled++;
            });
            engine.Update(1f);
            Assert.AreEqual(2, listenerCalled);
        }

        [Test]
        public void CanSubscribeToEvents()
        {
            engine.Subscribe("customEvent", arg => { });
        }
    }
}