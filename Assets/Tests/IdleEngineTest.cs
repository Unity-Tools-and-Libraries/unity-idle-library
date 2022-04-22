using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Configuration;
using io.github.thisisnozaku.idle.framework.Exceptions;
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
            var configuration = new EngineConfiguration();
            configuration.DeclareGlobalProperty("globalBoolean", true);
            configuration.DeclareGlobalProperty("globalBooleanNoStartingValue");

            configuration.DeclareGlobalProperty("globalNumber", BigDouble.One);
            configuration.DeclareGlobalProperty("globalNumberNoStartingValue");

            configuration.DeclareGlobalProperty("globalString", "startingValue");
            configuration.DeclareGlobalProperty("globalStringNoStartingValue");

            configuration.DeclareGlobalProperty("globalMapNoStartingValue");
            configuration.DeclareGlobalProperty("globalMap", new Dictionary<string, ValueContainerDefinition>()
            {
                { "foo", new ValueContainerDefinitionBuilder().WithStartingValue("bar").Build() }
            });

            configuration.DeclareGlobalProperty("incrementingNumberValue",
                new ValueContainerDefinitionBuilder().WithUpdater((engine, deltaTime, currentValue, parent) =>
                {
                    return (BigDouble)currentValue + BigDouble.One;
                })
                .Build());

            engine = new IdleEngine(configuration, null);
        }

        [Test]
        public void TryingToGetOrCreateAnEntityWithAnUndefinedTypeThrowsAnException()
        {
            Assert.Throws<UndefinedEntityException>(() => engine.GetOrCreateEntity("entity"));
        }

        [Test]
        public void TryingToGetUndeclaredGlobalPropertyThrowsAnException()
        {
            Assert.Throws<UndefinedPropertyException>(() =>
            {
                engine.GetGlobalProperty("boolean");
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
        public void MapGlobalPropertyWithDefaultReturnsDefault()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("globalMap");
            var expected = new Dictionary<string, ValueContainer>() {
                {"foo",
                    new ValueContainerDefinitionBuilder().WithStartingValue("bar").Build().CreateValueReference(engine) }
                };
            Assert.AreEqual(expected,
                propertyReference.ValueAsMap());
        }

        [Test]
        public void CallingUpdateOnEngineCallsUpdateOnAllValues()
        {
            ValueContainer propertyReference = engine.GetGlobalProperty("incrementingNumberValue");
            int listenerCalled = 0;
            propertyReference.Watch(v =>
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