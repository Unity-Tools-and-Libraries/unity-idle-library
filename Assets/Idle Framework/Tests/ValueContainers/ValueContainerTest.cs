using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueContainerTest : RequiresEngineTests
    {
        [SetUp]
        public void Setup()
        {
            InitializeEngine();
        }
        // Boolean
        [Test]
        public void GettingBoolAsBoolReturnsAsIs()
        {
            Assert.IsTrue(new ValueContainerDefinitionBuilder().WithStartingValue(true)
                .Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsFalse(new ValueContainerDefinitionBuilder().WithStartingValue(false)
                .Build().CreateValueReference(engine).ValueAsBool());
        }

        [Test]
        public void GettingNullAsBoolReturnsFalse()
        {
            Assert.IsFalse(new ValueContainerDefinitionBuilder().WithStartingValue((string)null)
                .Build().CreateValueReference(engine).ValueAsBool());
        }

        [Test]
        public void GettingNumberAsBool()
        {
            Assert.AreEqual(BigDouble.One, new ValueContainerDefinitionBuilder()
                .WithStartingValue(true)
                .Build().CreateValueReference(engine).ValueAsNumber());
            Assert.AreEqual(BigDouble.Zero, new ValueContainerDefinitionBuilder()
                .WithStartingValue(false)
                .Build().CreateValueReference(engine)
                .ValueAsNumber());
        }

        [Test]
        public void GettingMapAsBoolReturnsTrueIfNotNull()
        {
            Assert.IsFalse(new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsTrue(new ValueContainerDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueContainerDefinition>())
                .Build().CreateValueReference(engine)
                .ValueAsBool());
        }

        [Test]
        public void CanImplicitlyConvertToBool()
        {
            Assert.IsTrue(new ValueContainerDefinitionBuilder()
                .WithStartingValue(true)
                .Build().CreateValueReference(engine));
        }

        // Number
        [Test]
        public void GettingNumberAsNumberReturnsAsIs()
        {
            Assert.AreEqual(new BigDouble(100), new ValueContainerDefinitionBuilder()
                .WithStartingValue(new BigDouble(100))
                .Build().CreateValueReference(engine)
                .ValueAsNumber());
        }

        [Test]
        public void GettingNumberAsBoolReturnsTrueForNonZero()
        {
            Assert.IsTrue(new ValueContainerDefinitionBuilder().WithStartingValue(new BigDouble(1)).Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsTrue(new ValueContainerDefinitionBuilder().WithStartingValue(new BigDouble(-1)).Build().CreateValueReference(engine).ValueAsBool());
            Assert.IsFalse(new ValueContainerDefinitionBuilder().WithStartingValue(BigDouble.Zero).Build().CreateValueReference(engine).ValueAsBool());
        }

        [Test]
        public void GettingMapAsNumberReturnsZero()
        {
            Assert.AreEqual(BigDouble.Zero,
                new ValueContainerDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueContainerDefinition>())
                .Build().CreateValueReference(engine)
                .ValueAsNumber());
        }

        [Test]
        public void CanImplicitlyConvertToNumber()
        {
            Assert.AreEqual(BigDouble.One, (BigDouble)new ValueContainerDefinitionBuilder()
                .WithStartingValue(BigDouble.One)
                .Build().CreateValueReference(engine));
        }

        [Test]
        public void CanImplicitlyConvertToString()
        {
            Assert.AreEqual("true", (string)new ValueContainerDefinitionBuilder()
                .WithStartingValue("true")
                .Build().CreateValueReference(engine));
        }

        [Test]
        public void WatchingForChangesReceivesCurrentValueImmediately()
        {
            var valueReference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
            valueReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newValue => Assert.AreEqual(BigDouble.Zero, newValue));
        }

        [Test]
        public void ChangingValueOfWatchedValueNotifiesListeners()
        {
            var valueReference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
            int calls = 0;
            valueReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newValue =>
            {
                calls++;
                if (calls == 0)
                {
                    Assert.AreEqual(BigDouble.Zero, newValue);
                }
                else
                {
                    Assert.AreEqual(BigDouble.One, newValue);
                }
            });
            valueReference.Set(BigDouble.One);
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void RawValueReturnsActualValue()
        {
            var booleanReference = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            var numberReference = new ValueContainerDefinitionBuilder().WithStartingValue(BigDouble.One).Build().CreateValueReference(engine);
            var stringReference = new ValueContainerDefinitionBuilder().WithStartingValue("string").Build().CreateValueReference(engine);
            var mapReference = new ValueContainerDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueContainerDefinition>()).Build().CreateValueReference(engine);

            Assert.AreEqual(true, booleanReference.ValueAsRaw());
            Assert.AreEqual(BigDouble.One, numberReference.ValueAsRaw());
            Assert.AreEqual("string", stringReference.ValueAsRaw());
            Assert.AreEqual(new Dictionary<string, ValueContainer>(), mapReference.ValueAsRaw());
        }

//        [Test]
        public void CanWatchANonExistantKeyInAMap()
        {
            var mapReference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueContainerDefinition>()).Build().CreateValueReference(engine);
            var fooReference = mapReference.ValueAsMap()["foo"];
            var watchListenerCalled = false;
            fooReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newFoo =>
            {
                watchListenerCalled = true;
            });
            Assert.IsTrue(watchListenerCalled);
        }

        [Test]
        public void AssigningAValueInAMapUpdatesTheParent()
        {
            var mapReference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueContainerDefinition>()).Build().CreateValueReference(engine);
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            mapReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, updatedMap =>
            {
                watchListenerCalled++;
            });

            map["foo"] = new ValueContainer(mapReference, BigDouble.One);
            Assert.True(map is ParentNotifyingDictionary);
            Assert.AreEqual(1, watchListenerCalled);
        }

        [Test]
        public void WatchListenerReceivesMapValueWhenChildUpdates()
        {
            var mapReference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(new Dictionary<string, ValueContainerDefinition>()).Build().CreateValueReference(engine);
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            mapReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, updatedMap =>
            {
                Assert.IsNotNull(updatedMap as IDictionary<string, ValueContainer>);
                watchListenerCalled++;
            });
            map["foo"] = new ValueContainer(mapReference, BigDouble.One);
            Assert.AreEqual(1, watchListenerCalled);
        }

        [Test]
        public void ValueReferenceEqualsComparesUnderlyingValue()
        {
            var ref1 = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            var ref2 = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            Assert.AreEqual(ref1, ref2);
            var ref3 = new ValueContainerDefinitionBuilder().WithStartingValue("true").Build().CreateValueReference(engine);
            Assert.AreNotEqual(ref1, ref3);
        }

        [Test]
        public void EqualsComparingValueReferenceToAnyOtherTypeAlwaysFalse()
        {
            var ref1 = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            Assert.AreNotEqual(ref1, new Dictionary<string, string>());
        }

        [Test]
        public void ToStringDescribesContents()
        {
            var mapReference = new ValueContainerDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueContainerDefinition>()).Build().CreateValueReference(engine);
            Assert.AreEqual("Reference #1(containing map)", mapReference.ToString());

            var stringReference = new ValueContainerDefinitionBuilder().WithStartingValue("string").Build().CreateValueReference(engine);
            Assert.AreEqual("Reference #2(containing string)", stringReference.ToString());

            var boolReference = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            Assert.AreEqual("Reference #3(containing boolean)", boolReference.ToString());

            var numberReference = new ValueContainerDefinitionBuilder().WithStartingValue(BigDouble.One).Build().CreateValueReference(engine);
            Assert.AreEqual("Reference #4(containing number)", numberReference.ToString());
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToANumber()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(BigDouble.One).Build().CreateValueReference(engine);
            reference.Set(new BigDouble(2));
            Assert.AreEqual(reference.ValueAsNumber(), new BigDouble(2));
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToAString()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue("oldString").Build().CreateValueReference(engine);
            reference.Set("newString");
            Assert.AreEqual(reference.ValueAsString(), "newString");
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToAMap()
        {
            var initialDictionary = new Dictionary<string, ValueContainerDefinition>();
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(initialDictionary).Build().CreateValueReference(engine);
            var newDictionary = new Dictionary<string, ValueContainer>();
            reference.Set(newDictionary);
            Assert.AreEqual(reference.ValueAsMap(), newDictionary);
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToABool()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            reference.Set(false);
            Assert.IsFalse(reference.ValueAsBool());
        }

        [Test]
        public void TheStateOfAValueReferenceContainingABoolCanBeSaved()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(true).Build().CreateValueReference(engine);
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(new ValueContainer.Snapshot("1", true), serialized);
        }

        [Test]
        public void TheStateOfAValueReferenceContainingANumerCanBeSaved()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(new BigDouble(100)).Build().CreateValueReference(engine);
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(new ValueContainer.Snapshot("1", new BigDouble(100)), serialized);
        }

        [Test]
        public void TheStateOfAValueReferenceContainingAStringCanBeSaved()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue("astring").Build().CreateValueReference(engine);
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(serialized.value, "astring");
            Assert.AreEqual(serialized.internalId, reference.Id);
        }

        [Test]
        public void TheStateOfAValueReferenceContainingADictionaryCanBeSaved()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueContainerDefinition>()).Build().CreateValueReference(engine);
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(serialized, new ValueContainer.Snapshot("1", new ValueContainer(null, new Dictionary<string, ValueContainer>())));
        }

        [Test]
        public void TheStateOfAValueReferenceContainingAStringCanBeRecursivelySaved()
        {
            var reference = new ValueContainerDefinitionBuilder().WithStartingValue(new Dictionary<string, ValueContainerDefinition>()
            {
                { "string", "aString" },
                { "number", new BigDouble(100) },
                { "bool", true },
                { "map", new Dictionary<string, ValueContainerDefinition>() {
                    { "nestedString", "string" },
                    { "nestedNumber", new BigDouble(10) },
                    { "nestedBool", true }
                }}
            }).Build().CreateValueReference(engine);
            var serialized = reference.GetSnapshot();
            var durr = reference.ValueAsMap();
            Debug.Log(durr.Count);
            var nestedMap = new Dictionary<string, ValueContainer.Snapshot>() {
                { "nestedString" , new ValueContainer.Snapshot(reference.ValueAsMap()["map"].ValueAsMap()["nestedString"].Id, "string") },
                { "nestedNumber", new ValueContainer.Snapshot(reference.ValueAsMap()["map"].ValueAsMap()["nestedNumber"].Id, new BigDouble(10)) },
                { "nestedBool", new ValueContainer.Snapshot(reference.ValueAsMap()["map"].ValueAsMap()["nestedBool"].Id, true) }
            };
            var expected = new ValueContainer.Snapshot("8", new Dictionary<string, ValueContainer.Snapshot>()
            {
                { "string", new ValueContainer.Snapshot("1", "aString") },
                { "number", new ValueContainer.Snapshot("2", new BigDouble(100)) },
                { "bool", new ValueContainer.Snapshot("3", true) },
                {
                    "map", new ValueContainer.Snapshot("7", nestedMap)
                }
            });

            Assert.AreEqual(expected, serialized);
            Assert.AreEqual(serialized.internalId, reference.Id);
        }

        [Test]
        public void TheStateOfAValueReferenceCanBeRestoredFromABooleanSnapshot()
        {
            var reference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
            reference.RestoreFromSnapshot(engine, new ValueContainer.Snapshot("1", true));
            Assert.AreEqual(true, reference.ValueAsBool());
        }

        [Test]
        public void TheStateOfAValueReferenceCanBeRestoredFromANumberSnapshot()
        {
            var reference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
            reference.RestoreFromSnapshot(engine, new ValueContainer.Snapshot("1", new BigDouble(11)));
            Assert.AreEqual(new BigDouble(11), reference.ValueAsNumber());
        }

        [Test]
        public void TheStateOfAValueReferenceCanBeRestoredFromAMapSnapshot()
        {
            var reference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
            reference.RestoreFromSnapshot(engine, new ValueContainer.Snapshot("1", new ValueContainer(null, new Dictionary<string, ValueContainer>())));
            Assert.AreEqual(new Dictionary<string, ValueContainer>(), reference.ValueAsMap());
        }

        [Test]
        public void SettingIdMultipleTimesThrowsError()
        {
            var reference = new ValueContainerDefinitionBuilder().Build().CreateValueReference(engine);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                reference.Id = "123";
            });
        }

        [Test]
        public void HashcodeSameAsValue()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue("string")
                .Build().CreateValueReference(engine);

            Assert.AreEqual("1".GetHashCode() ^ "string".GetHashCode(), reference.GetHashCode());
            reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(BigDouble.One)
                .Build().CreateValueReference(engine);
            Assert.AreEqual("2".GetHashCode() ^ BigDouble.One.GetHashCode(), reference.GetHashCode());
        }

        [Test]
        public void RestorFromSnapshotFailsWithDifferentIds()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue("string")
                .Build().CreateValueReference(engine);
            var snapshot = reference.GetSnapshot();
            reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue("string")
                .Build().CreateValueReference(engine);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                reference.RestoreFromSnapshot(engine, snapshot);
            });
        }

        [Test]
        public void CanSpecifyACalculatingFunction()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, deltaTime, val, parent, mds) => (BigDouble)val + 1)
                .Build().CreateValueReference(engine);
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(1f);
                Assert.AreEqual(new BigDouble(i), reference.ValueAsNumber());
            }
        }

        [Test]
        public void SettingValueOfCalculatedValueThrowsError()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => val + 1)
                .Build().CreateValueReference(engine);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                reference.Set(1);
            });
        }

        [Test]
        public void ReturningNullFromUpdaterThrowsError()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => null)
                .Build().CreateValueReference(engine);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Update(1f);
            });
        }

        [Test]
        public void ReturningIntFromUpdaterBecomesBigDouble()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => 1)
                .Build().CreateValueReference(engine);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningFloatFromUpdaterBecomesBigDouble()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => 1f)
                .Build().CreateValueReference(engine);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningLongFromUpdaterBecomesBigDouble()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => 1L)
                .Build().CreateValueReference(engine);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningDoubleFromUpdaterBecomesBigDouble()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => 1.0)
                .Build().CreateValueReference(engine);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningInvalidValueFromUpdaterThrowsException()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithStartingValue(0)
                .WithUpdater((eng, val, deltaTime, parent, mds) => new string[] { })
                .Build().CreateValueReference(engine);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Update(1f);
            });
        }
    }
}