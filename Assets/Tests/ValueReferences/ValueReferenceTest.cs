using BreakInfinity;
using IdleFramework.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace IdleFramework.Tests
{
    public class ValueReferenceTest
    {
        private IdleEngine engine;
        [SetUp]
        public void Setup()
        {
            engine = new IdleEngine(null, null);
        }
        // Boolean
        [Test]
        public void GettingBoolAsBoolReturnsAsIs()
        {
            Assert.IsTrue(new ValueReferenceDefinitionBuilder().WithStartingValue(true)
                .Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsFalse(new ValueReferenceDefinitionBuilder().WithStartingValue(false)
                .Build().CreateValueReference(engine).ValueAsBool());
        }

        [Test]
        public void GettingNumberAsBool()
        {
            Assert.AreEqual(BigDouble.One, new ValueReferenceDefinitionBuilder()
                .WithStartingValue(true)
                .Build().CreateValueReference(engine).ValueAsNumber());
            Assert.AreEqual(BigDouble.Zero, new ValueReferenceDefinitionBuilder()
                .WithStartingValue(false)
                .Build().CreateValueReference(engine)
                .ValueAsNumber());
        }

        [Test]
        public void GettingMapAsBoolReturnsTrueIfNotNull()
        {
            Assert.IsFalse(new ValueReferenceDefinitionBuilder().Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsTrue(new ValueReferenceDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueReferenceDefinition>())
                .Build().CreateValueReference(engine)
                .ValueAsBool());
        }

        // Number
        [Test]
        public void GettingNumberAsNumberReturnsAsIs()
        {
            Assert.AreEqual(new BigDouble(100), new ValueReferenceDefinitionBuilder()
                .WithStartingValue(new BigDouble(100))
                .Build().CreateValueReference(engine)
                .ValueAsNumber());
        }

        [Test]
        public void GettingNumberAsBoolReturnsTrueForNonZero()
        {
            Assert.IsTrue(new ValueReferenceDefinitionBuilder().WithStartingValue(new BigDouble(1)).Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsTrue(new ValueReferenceDefinitionBuilder().WithStartingValue(new BigDouble(-1)).Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsFalse(new ValueReferenceDefinitionBuilder().WithStartingValue(BigDouble.Zero).Build().CreateValueReference(engine).ValueAsBool());
        }

        [Test]
        public void GettingMapAsNumberReturnsZero()
        {
            Assert.AreEqual(BigDouble.Zero,
                new ValueReferenceDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueReferenceDefinition>())
                .Build().CreateValueReference(engine)
                .ValueAsNumber());
        }

        [Test]
        public void WatchingForChangesReceivesCurrentValueImmediately()
        {
            var valueReference = new ValueReferenceDefinitionBuilder().Build().CreateValueReference(engine);
            valueReference.Watch(newValue => Assert.AreEqual(BigDouble.Zero, newValue));
        }

        [Test]
        public void ChangingValueOfWatchedValueNotifiesListeners()
        {
            var valueReference = new ValueReferenceDefinitionBuilder().Build().CreateValueReference(engine);
            int calls = 0;
            valueReference.Watch(newValue =>
            {
                calls++;
                if (calls == 1)
                {
                    Assert.AreEqual(BigDouble.Zero, newValue);
                }
                else
                {
                    Assert.AreEqual(BigDouble.One, newValue);
                }
            });
            valueReference.Set(BigDouble.One);
            Assert.AreEqual(2, calls);
        }

        [Test]
        public void RawValueReturnsActualValue()
        {
            var booleanReference = new ValueReferenceDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            var numberReference = new ValueReferenceDefinitionBuilder().WithStartingValue(BigDouble.One).Build().CreateValueReference(engine);
            var stringReference = new ValueReferenceDefinitionBuilder().WithStartingValue("string").Build().CreateValueReference(engine);
            var mapReference = new ValueReferenceDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueReferenceDefinition>()).Build().CreateValueReference(engine);

            Assert.AreEqual(true, booleanReference.ValueAsRaw());
            Assert.AreEqual(BigDouble.One, numberReference.ValueAsRaw());
            Assert.AreEqual("string", stringReference.ValueAsRaw());
            Assert.AreEqual(new Dictionary<string, ValueReference>(), mapReference.ValueAsRaw());
        }

        [Test]
        public void CanWatchANonExistantKeyInAMap()
        {
            var mapReference = new ValueReferenceDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueReferenceDefinition>()).Build().CreateValueReference(engine);
            var fooReference = mapReference.ValueAsMap()["foo"];
            var watchListenerCalled = false;
            fooReference.Watch(newFoo =>
            {
                watchListenerCalled = true;
            });
            Assert.IsTrue(watchListenerCalled);
        }

        [Test]
        public void AssigningAValueInAMapUpdatesTheParent()
        {
            var mapReference = new ValueReferenceDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueReferenceDefinition>()).Build().CreateValueReference(engine);
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            mapReference.Watch(updatedMap =>
            {
                watchListenerCalled++;
            });

            map["foo"].Set(BigDouble.One);
            Assert.AreEqual(3, watchListenerCalled);
        }

        [Test]
        public void WatchListenerReceivesMapValueWhenChildUpdates()
        {
            var mapReference = new ValueReferenceDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueReferenceDefinition>()).Build().CreateValueReference(engine);
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            mapReference.Watch(updatedMap =>
            {
                Assert.IsNotNull(updatedMap as IDictionary<string, ValueReference>);
                watchListenerCalled++;
            });

            map["foo"].Set(BigDouble.One);
            Assert.AreEqual(3, watchListenerCalled);
        }
    }
}