using BreakInfinity;
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
            Assert.IsTrue(engine.CreateValueContainer(true).ValueAsBool());
            Assert.IsFalse(engine.CreateValueContainer(false).ValueAsBool());
        }

        [Test]
        public void GettingNullAsBoolReturnsFalse()
        {
            Assert.IsFalse(engine.CreateValueContainer((string)null).ValueAsBool());
        }

        [Test]
        public void GettingNumberAsBool()
        {
            Assert.AreEqual(BigDouble.One, engine.CreateValueContainer(true).ValueAsNumber());
            Assert.AreEqual(BigDouble.Zero, engine.CreateValueContainer(false).ValueAsNumber());
        }

        [Test]
        public void GettingMapAsBoolReturnsTrueIfNotNull()
        {
            Assert.IsFalse(engine.CreateValueContainer(null as Dictionary<string, ValueContainer>).ValueAsBool());
            Assert.IsTrue(engine.CreateValueContainer(new Dictionary<string, ValueContainer>()).ValueAsBool());
        }

        [Test]
        public void CanImplicitlyConvertToBool()
        {
            Assert.IsTrue(engine.CreateValueContainer(true));
        }

        // Number
        [Test]
        public void GettingNumberAsNumberReturnsAsIs()
        {
            Assert.AreEqual(new BigDouble(100), engine.CreateValueContainer(new BigDouble(100)).ValueAsNumber());
        }

        [Test]
        public void GettingNumberAsBoolReturnsTrueForNonZero()
        {
            Assert.IsTrue(engine.CreateValueContainer(new BigDouble(1)).ValueAsBool());
            Assert.IsTrue(engine.CreateValueContainer(new BigDouble(-1)).ValueAsBool());
            Assert.IsFalse(engine.CreateValueContainer(BigDouble.Zero).ValueAsBool());
        }

        [Test]
        public void GettingMapAsNumberReturnsZero()
        {
            Assert.AreEqual(BigDouble.Zero,
                engine.CreateValueContainer(new Dictionary<string, ValueContainer>()).ValueAsNumber());
        }

        [Test]
        public void CanImplicitlyConvertToNumber()
        {
            Assert.AreEqual(BigDouble.One, (BigDouble)engine.CreateValueContainer(BigDouble.One));
        }

        [Test]
        public void CanImplicitlyConvertToString()
        {
            Assert.AreEqual("true", (string)engine.CreateValueContainer("true"));
        }

        [Test]
        public void WatchingForChangesReceivesCurrentValueImmediately()
        {
            var valueReference = engine.CreateValueContainer();
            bool called = false;
            valueReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newValue => called = true);
            Assert.IsTrue(called);
        }

        [Test]
        public void ChangingValueOfWatchedValueNotifiesListeners()
        {
            var valueReference = engine.CreateValueContainer();
            int calls = 0;
            valueReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newValue =>
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
            var booleanReference = engine.CreateValueContainer(true);
            var numberReference = engine.CreateValueContainer(BigDouble.One);
            var stringReference = engine.CreateValueContainer("string");
            var mapReference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());

            Assert.AreEqual(true, booleanReference.ValueAsRaw());
            Assert.AreEqual(BigDouble.One, numberReference.ValueAsRaw());
            Assert.AreEqual("string", stringReference.ValueAsRaw());
            Assert.AreEqual(new Dictionary<string, ValueContainer>(), mapReference.ValueAsRaw());
        }

//        [Test]
        public void CanWatchANonExistantKeyInAMap()
        {
            var mapReference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
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
            var mapReference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            mapReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, updatedMap =>
            {
                watchListenerCalled++;
            });

            map["foo"] = engine.CreateValueContainer(BigDouble.One);
            Assert.True(map is ParentNotifyingDictionary);
            Assert.AreEqual(3, watchListenerCalled);
        }

        [Test]
        public void WatchListenerReceivesMapValueWhenChildUpdates()
        {
            var mapReference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            mapReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, updatedMap =>
            {
                Assert.IsNotNull(updatedMap as IDictionary<string, ValueContainer>);
                watchListenerCalled++;
            });
            map["foo"] = engine.CreateValueContainer(BigDouble.One);
            Assert.AreEqual(3, watchListenerCalled); // 1 time for subscriping to mapReference, 1 time when parent subscribes to child container, 3 when value is changed.
        }

        [Test]
        public void ValueReferenceEqualsComparesUnderlyingValue()
        {
            var ref1 = engine.CreateValueContainer(true);
            var ref2 = engine.CreateValueContainer(true);
            Assert.AreEqual(ref1, ref2);
            var ref3 = engine.CreateValueContainer("true");
            Assert.AreNotEqual(ref1, ref3);
        }

        [Test]
        public void EqualsComparingValueReferenceToAnyOtherTypeAlwaysFalse()
        {
            var ref1 = engine.CreateValueContainer(true);
            Assert.AreNotEqual(ref1, new Dictionary<string, string>());
        }

        [Test]
        public void ToStringDescribesContents()
        {
            var mapReference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            Assert.AreEqual("Reference #1(containing map)", mapReference.ToString());

            var stringReference = engine.CreateValueContainer("string");
            Assert.AreEqual("Reference #2(containing string)", stringReference.ToString());

            var boolReference = engine.CreateValueContainer(true);
            Assert.AreEqual("Reference #3(containing boolean)", boolReference.ToString());

            var numberReference = engine.CreateValueContainer(BigDouble.One);
            Assert.AreEqual("Reference #4(containing number)", numberReference.ToString());
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToANumber()
        {
            var reference = engine.CreateValueContainer(BigDouble.One);
            reference.Set(new BigDouble(2));
            Assert.AreEqual(reference.ValueAsNumber(), new BigDouble(2));
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToAString()
        {
            var reference = engine.CreateValueContainer("oldString");
            reference.Set("newString");
            Assert.AreEqual(reference.ValueAsString(), "newString");
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToAMap()
        {
            var initialDictionary = new Dictionary<string, ValueContainer>();
            var reference = engine.CreateValueContainer(initialDictionary);
            var newDictionary = new Dictionary<string, ValueContainer>();
            reference.Set(newDictionary);
            Assert.AreEqual(reference.ValueAsMap(), newDictionary);
        }

        [Test]
        public void CanSetTheValueContainedByAReferenceToABool()
        {
            var reference = engine.CreateValueContainer(true);
            reference.Set(false);
            Assert.IsFalse(reference.ValueAsBool());
        }

        [Test]
        public void TheStateOfAValueReferenceContainingABoolCanBeSaved()
        {
            var reference = engine.CreateValueContainer(true);
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(new ValueContainer.Snapshot("1", reference), serialized);
        }

        [Test]
        public void TheStateOfAValueReferenceContainingANumerCanBeSaved()
        {
            var reference = engine.CreateValueContainer(new BigDouble(100));
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(new ValueContainer.Snapshot("1", reference), serialized);
        }

        [Test]
        public void TheStateOfAValueReferenceContainingAStringCanBeSaved()
        {
            var reference = engine.CreateValueContainer("astring");
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(serialized.value, "astring");
            Assert.AreEqual(serialized.internalId, reference.Id);
        }

        [Test]
        public void TheStateOfAValueReferenceContainingADictionaryCanBeSaved()
        {
            var reference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            var serialized = reference.GetSnapshot();
            Assert.AreEqual(serialized, new ValueContainer.Snapshot("1", engine.CreateValueContainer(new Dictionary<string, ValueContainer>())));
        }

        [Test]
        public void TheStateOfAValueReferenceContainingAStringCanBeRecursivelySaved()
        {
            var reference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()
            {
                { "string", engine.CreateValueContainer("aString") },
                { "number", engine.CreateValueContainer(new BigDouble(100)) },
                { "bool",   engine.CreateValueContainer(true) },
                { "map", engine.CreateValueContainer(new Dictionary<string, ValueContainer>() {
                    { "nestedString", engine.CreateValueContainer("string") },
                    { "nestedNumber", engine.CreateValueContainer(new BigDouble(10)) },
                    { "nestedBool", engine.CreateValueContainer(true) }
                })}
            });
            var serialized = reference.GetSnapshot();
            var durr = reference.ValueAsMap();
            Debug.Log(durr.Count);
            var nestedMap = new Dictionary<string, ValueContainer.Snapshot>() {
                { "nestedString" , new ValueContainer.Snapshot(reference.ValueAsMap()["map"].ValueAsMap()["nestedString"].Id, 
                    engine.CreateValueContainer("string")) },
                { "nestedNumber", new ValueContainer.Snapshot(reference.ValueAsMap()["map"].ValueAsMap()["nestedNumber"].Id, 
                    engine.CreateValueContainer(new BigDouble(10))) },
                { "nestedBool", new ValueContainer.Snapshot(reference.ValueAsMap()["map"].ValueAsMap()["nestedBool"].Id, 
                    engine.CreateValueContainer(true)) }
            };
            var expected = new ValueContainer.Snapshot("8", new Dictionary<string, ValueContainer.Snapshot>()
            {
                { "string", new ValueContainer.Snapshot("1", engine.CreateValueContainer("aString")) },
                { "number", new ValueContainer.Snapshot("2", engine.CreateValueContainer(new BigDouble(100))) },
                { "bool", new ValueContainer.Snapshot("3", engine.CreateValueContainer(true)) },
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
            var reference = engine.CreateValueContainer();
            reference.RestoreFromSnapshot(engine, new ValueContainer.Snapshot("1", engine.CreateValueContainer(true)));
            Assert.AreEqual(true, reference.ValueAsBool());
        }

        [Test]
        public void TheStateOfAValueReferenceCanBeRestoredFromANumberSnapshot()
        {
            var reference = engine.CreateValueContainer();
            reference.RestoreFromSnapshot(engine, new ValueContainer.Snapshot("1", engine.CreateValueContainer(new BigDouble(11))));
            Assert.AreEqual(new BigDouble(11), reference.ValueAsNumber());
        }

        [Test]
        public void TheStateOfAValueReferenceCanBeRestoredFromAMapSnapshot()
        {
            var reference = engine.CreateValueContainer();
            reference.RestoreFromSnapshot(engine, new ValueContainer.Snapshot("1", engine.CreateValueContainer(new Dictionary<string, ValueContainer>())));
            Assert.AreEqual(new Dictionary<string, ValueContainer>(), reference.ValueAsMap());
        }

        [Test]
        public void SettingIdMultipleTimesThrowsError()
        {
            var reference = engine.CreateValueContainer();
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                reference.Id = "123";
            });
        }

        [Test]
        public void HashcodeSameAsValue()
        {
            var reference = engine.CreateValueContainer("string");

            Assert.AreEqual("1".GetHashCode() ^ "string".GetHashCode(), reference.GetHashCode());
            reference = engine.CreateValueContainer(BigDouble.One);
            Assert.AreEqual("2".GetHashCode() ^ BigDouble.One.GetHashCode(), reference.GetHashCode());
        }

        [Test]
        public void RestorFromSnapshotFailsWithDifferentIds()
        {
            var reference = engine.CreateValueContainer("string");
            var snapshot = reference.GetSnapshot();
            reference = engine.CreateValueContainer("string");
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                reference.RestoreFromSnapshot(engine, snapshot);
            });
        }

        [Test]
        public void CanSpecifyACalculatingFunction()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => (BigDouble)val + 1);
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(1f);
                Assert.AreEqual(new BigDouble(i), reference.ValueAsNumber());
            }
        }

        [Test]
        public void SettingValueOfCalculatedValueThrowsError()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => (BigDouble)val + 1);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                reference.Set(1);
            });
        }

        [Test]
        public void ReturningNullFromUpdaterThrowsError()
        {
            var reference = engine.CreateValueContainer(0, updater : (eng, deltaTime, val, cont, mds) => null);
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Update(1f);
            });
        }

        [Test]
        public void ReturningIntFromUpdaterBecomesBigDouble()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => 1);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningFloatFromUpdaterBecomesBigDouble()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => 1f);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningLongFromUpdaterBecomesBigDouble()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => 1L);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningDoubleFromUpdaterBecomesBigDouble()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => 1.0);
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningInvalidValueFromUpdaterThrowsException()
        {
            var reference = engine.CreateValueContainer(0, updater: (eng, deltaTime, val, cont, mds) => new string[] { });
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Update(1f);
            });
        }
    }
}