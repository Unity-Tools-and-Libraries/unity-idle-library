using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static io.github.thisisnozaku.idle.framework.Engine.IdleEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class ValueContainerTest : RequiresEngineTests
    {
        [SetUp]
        public void Setup()
        {
            InitializeEngine();
        }

        //[Test]
        public void CanWatchANonExistantKeyInAMap()
        {
            var mapReference = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            var fooReference = mapReference.ValueAsMap()["foo"];
            var watchListenerCalled = false;
            fooReference.Subscribe("", ValueChangedEvent.EventName, (ie, c, ev) =>
            {
                return watchListenerCalled = true;
            });
            Assert.IsTrue(watchListenerCalled);
        }

        [Test]
        public void CanUnsubscribeFromEventsOnContainer()
        {
            engine.Start();
            int callCount = 0;
            engine.RegisterMethod("method", (ie, vc, ev) => {
                callCount++;
                return null;
            });
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>(), path: "path");
            var subscription = container.Subscribe("event", "event", "method");
            container.NotifyImmediately("event", null, null, null);
            container.Unsubscribe(subscription);
            container.NotifyImmediately("event", null, null, null);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CanSetPropertyIfHoldingDictionary()
        {
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>(), path: "path");
            container["foo"] = engine.CreateValueContainer("bar");
            Assert.AreEqual("bar", container.ValueAsMap()["foo"].ValueAsString());
        }

        [Test]
        public void CanGetPropertyIfHoldingDictionary()
        {
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()
            {
                { "foo", engine.CreateValueContainer("bar") }
            }, path: "path");
            Assert.AreEqual("bar", container["foo"].ValueAsString());
        }

        [Test]
        public void CannotSetPropertyIfNotHoldingDictionary()
        {
            var container = engine.CreateValueContainer(true, path: "path");

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                container["foo"] = engine.CreateValueContainer("bar");
            });
        }

        [Test]
        public void GettingPropertyViaIndexReturnsNullIfNotContainingDictionary()
        {
            var container = engine.CreateValueContainer(true, path: "path");
            
            Assert.IsNull(container["foo"]);
        }

        [Test]
        public void WatchListenerReceivesMapValueWhenChildUpdates()
        {
            var mapReference = engine.SetProperty("path", new Dictionary<string, ValueContainer>());
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            engine.RegisterMethod("method", (ie, c, args) =>
            {
                Assert.AreEqual("path.foo", args[1] as string);

                watchListenerCalled++;
                return null;
            });
            engine.Start();
            mapReference.Subscribe("path", ChildValueChangedEvent.EventName, "method");
            map["foo"] = engine.CreateValueContainer(BigDouble.One);
            Assert.AreEqual(1, watchListenerCalled);
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
        public void CanSetUpdateMethodByName()
        {
            engine.Start();
            engine.RegisterMethod("method", (ie, cv, ev) =>
            {
                return 1;
            });
            var container = engine.SetProperty("path");
            engine.Update(1f);
            Assert.IsNull(container.ValueAsRaw());
            container.SetUpdater("method");
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(1), container.ValueAsNumber());
        }

        [Test]
        public void CanSetUpdateMethodByMethod()
        {
            engine.Start();
            IdleEngine.UserMethod listener = (ie, cv, ev) =>
            {
                return 1;
            };
            engine.RegisterMethod(listener);
            var container = engine.SetProperty("path");
            engine.Update(1f);
            Assert.IsNull(container.ValueAsRaw());
            container.SetUpdater(listener);
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(1), container.ValueAsNumber());
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
            var mapReference = engine.SetProperty("1", new Dictionary<string, ValueContainer>());
            Assert.AreEqual("Reference #1 @1: (containing map)", mapReference.ToString());

            var stringReference = engine.SetProperty("2", "aString");
            Assert.AreEqual("Reference #2 @2: (containing string 'aString')", stringReference.ToString());

            var boolReference = engine.SetProperty("3", true);
            Assert.AreEqual("Reference #3 @3: (containing boolean True)", boolReference.ToString());

            var numberReference = engine.SetProperty("4", BigDouble.One);
            Assert.AreEqual("Reference #4 @4: (containing number 1)", numberReference.ToString());
        }

        [Test]
        public void HashcodeSameAsValue()
        {
            var reference = engine.SetProperty("1", "string");

            Assert.AreEqual("1".GetHashCode() ^ "string".GetHashCode(), reference.GetHashCode());
            reference = engine.SetProperty("2", BigDouble.One);
            Assert.AreEqual("2".GetHashCode() ^ BigDouble.One.GetHashCode(), reference.GetHashCode());
        }

        [Test]
        public void CanSpecifyACalculatingFunction()
        {
            engine.RegisterMethod("method", (eng, cont, ev) => (BigDouble)(ev[0] as ValueContainerWillUpdateEvent).PreviousValue + 1);
            var reference = engine.SetProperty("path", 0, updater: "method");
            engine.Start();
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(1f);
                Assert.AreEqual(new BigDouble(i), reference.ValueAsNumber());
            }
        }

        [Test]
        public void CanGetChildPath()
        {

        }

        [Test]
        public void ReturningIntFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, cont, ev) => 1);
            var reference = engine.SetProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningFloatFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, cont, ev) => 1f);
            var reference = engine.SetProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningLongFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, cont, ev) => 1L);
            var reference = engine.SetProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningDoubleFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, cont, ev) => 1.0);
            var reference = engine.SetProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningInvalidValueFromUpdaterThrowsException()
        {
            engine.RegisterMethod("method", (eng, cont, ev) => new string[] { });
            var reference = engine.SetProperty("path", 0, updater: "method");
            engine.Start();
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Update(1f);
            });
        }

        [Test]
        public void GettingBoolFromUnregisteredContainerLogsError()
        {
            LogAssert.Expect(LogType.Error, "ValueContainer is not ready to be used; it must be assigned to a global property in the engine or a descendent of one before use.");
            var container = engine.CreateValueContainer(path: null);
            container.ValueAsBool();
        }

        [Test]
        public void InterceptorMethodCalledBeforeSettingValue()
        {
            int call = 0;
            engine.RegisterMethod("interceptor", (ie, vc, ev) => {
                call++;
                return null;
            });
            engine.SetProperty("path", interceptor: "interceptor")
                .Set(1);
            Assert.AreEqual(2, call);
        }

        [Test]
        public void CanSetInterceptorMethodByName()
        {
            engine.SetProperty("path").SetInterceptor("intercept");
        }

        [Test]
        public void CanSetInterceptorMethod()
        {
            UserMethod listener = (ie, vc, ev) => {
                return null;
            };
            engine.SetProperty("path").SetInterceptor(listener);
        }
    }
}