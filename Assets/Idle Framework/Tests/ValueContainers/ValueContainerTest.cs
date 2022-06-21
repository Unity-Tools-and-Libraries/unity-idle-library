using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
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
            fooReference.Subscribe("", ValueChangedEvent.EventName, (ie, ev) =>
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
            engine.RegisterMethod("method", (ie, ev) =>
            {
                callCount++;
                return null;
            });
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            var subscription = container.Subscribe("event", "event", "method");
            container.NotifyImmediately("event", null, null, null);
            container.Unsubscribe(subscription);
            container.NotifyImmediately("event", null, null, null);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CanSetPropertyIfHoldingDictionary()
        {
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>());
            container["foo"] = engine.CreateValueContainer("bar");
            Assert.AreEqual("bar", container.ValueAsMap()["foo"].ValueAsString());
        }

        [Test]
        public void CanGetPropertyIfHoldingDictionary()
        {
            var container = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()
            {
                { "foo", engine.CreateValueContainer("bar") }
            });
            Assert.AreEqual("bar", container["foo"].ValueAsString());
        }

        [Test]
        public void CannotSetPropertyIfNotHoldingDictionary()
        {
            var container = engine.CreateValueContainer(true);

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                container["foo"] = engine.CreateValueContainer("bar");
            });
        }

        [Test]
        public void GettingPropertyViaIndexReturnsNullIfNotContainingDictionary()
        {
            var container = engine.CreateValueContainer(true);

            Assert.IsNull(container["foo"]);
        }

        [Test]
        public void WatchListenerReceivesMapValueWhenChildUpdates()
        {
            var mapReference = engine.CreateProperty("path", new Dictionary<string, ValueContainer>());
            var map = mapReference.ValueAsMap();
            int watchListenerCalled = 0;
            engine.RegisterMethod("method", (ie, args) =>
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
            engine.RegisterMethod("method", (ie, ev) =>
            {
                return 1;
            });
            var container = engine.CreateProperty("path");
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
            IdleEngine.UserMethod listener = (ie, ev) =>
            {
                return 1;
            };
            engine.RegisterMethod(listener);
            var container = engine.CreateProperty("path");
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
            var mapReference = engine.CreateProperty("one", new Dictionary<string, ValueContainer>());
            Assert.AreEqual("Reference #1 @one: (containing map)", mapReference.ToString());

            var stringReference = engine.CreateProperty("two", "aString");
            Assert.AreEqual("Reference #2 @two: (containing string aString)", stringReference.ToString());

            var boolReference = engine.CreateProperty("three", true);
            Assert.AreEqual("Reference #3 @three: (containing bool True)", boolReference.ToString());

            var numberReference = engine.CreateProperty("four", BigDouble.One);
            Assert.AreEqual("Reference #4 @four: (containing number 1)", numberReference.ToString());
        }


        public void HashcodeSameAsValue()
        {
            var reference = engine.CreateProperty("1", "string");

            Assert.AreEqual("1".GetHashCode() ^ "string".GetHashCode(), reference.GetHashCode());
            reference = engine.CreateProperty("2", BigDouble.One);
            Assert.AreEqual("2".GetHashCode() ^ BigDouble.One.GetHashCode(), reference.GetHashCode());
        }

        [Test]
        public void CanSpecifyACalculatingFunction()
        {
            engine.RegisterMethod("method", (eng, ev) =>
            {
                return (BigDouble)ev[2] + 1;
            });
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            for (int i = 1; i <= 5; i++)
            {
                engine.Update(1f);
                Assert.AreEqual(new BigDouble(i), reference.ValueAsNumber());
            }
        }

        [Test]
        public void ReturningIntFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, ev) => 1);
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningFloatFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, ev) => 1f);
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningLongFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, ev) => 1L);
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningDoubleFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, ev) => 1.0);
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(BigDouble.One, reference.ValueAsNumber());
        }

        [Test]
        public void ReturningListFromUpdaterBecomesBigDouble()
        {
            engine.RegisterMethod("method", (eng, ev) => new List<ValueContainer>());
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            engine.Update(1f);
            Assert.AreEqual(new List<ValueContainer>(), reference.ValueAsList());
        }

        [Test]
        public void ReturningInvalidValueFromUpdaterThrowsException()
        {
            engine.RegisterMethod("method", (eng, ev) => new string[] { });
            var reference = engine.CreateProperty("path", 0, updater: "method");
            engine.Start();
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.Update(1f);
            });
        }


        public void GettingBoolFromUnregisteredContainerLogsError()
        {
            LogAssert.Expect(LogType.Error, "[engine.internal.container] ValueContainer is not ready to be used; it must be assigned to a global property in the engine or a descendent of one before use.");
            var container = engine.CreateValueContainer();
            container.ValueAsBool();
        }

        [Test]
        public void InterceptorMethodCalledBeforeSettingValue()
        {
            int call = 0;
            engine.RegisterMethod("interceptor", (ie, ev) =>
            {
                call++;
                return null;
            });
            engine.CreateProperty("path", interceptor: "interceptor")
                .Set(1);
            Assert.AreEqual(2, call);
        }

        [Test]
        public void CanSetInterceptorMethodByName()
        {
            engine.CreateProperty("path").SetInterceptor("intercept");
        }

        [Test]
        public void CanSetInterceptorMethod()
        {
            UserMethod listener = (ie, ev) =>
            {
                return null;
            };
            engine.CreateProperty("path").SetInterceptor(listener);
        }

        [Test]
        public void CallingGetValueRepeatedlyDoesNotReapplyModifiers()
        {
            engine.ConfigureLogging("engine.internal.modifier", LogType.Log);
            engine.ConfigureLogging("engine.internal.container.cache", LogType.Log);
            var container = engine.CreateProperty("path");
            var modifier = new CallCountingModifier(new AdditiveValueModifier("id", "modifier", 1));
            container.AddModifier(modifier);
            for (int i = 0; i < 10; i++)
            {
                container.ValueAsNumber();
            }
            Assert.AreEqual(1, modifier.applyCallCount);
        }

        public class CallCountingModifier : IContainerModifier
        {
            private IContainerModifier wrappedModifier;
            public int applyCallCount;
            public int generateContextCallCount;
            public int onAddCallCount;
            public int onRemovalCallCount;
            public int triggerCallCount;

            public string Id => wrappedModifier.Id;

            public Dictionary<string, object> Properties => wrappedModifier.Properties;

            public string Source => wrappedModifier.Source;

            public bool IsCached => wrappedModifier.IsCached;

            public int Order => wrappedModifier.Order;

            public CallCountingModifier(IContainerModifier wrappedModifier)
            {
                this.wrappedModifier = wrappedModifier;
            }

            public object Apply(IdleEngine engine, ValueContainer container, object input)
            {
                applyCallCount++;
                return wrappedModifier.Apply(engine, container, input);
            }

            public void OnAdd(IdleEngine engine, ValueContainer container)
            {
                onAddCallCount++;
                wrappedModifier.OnAdd(engine, container);
            }

            public void OnRemove(IdleEngine engine, ValueContainer container)
            {
                onRemovalCallCount++;
                wrappedModifier.OnRemove(engine, container);
            }

            public void Trigger(IdleEngine engine, string eventName, IDictionary<string, object> context = null)
            {
                triggerCallCount++;
                wrappedModifier.Trigger(engine, eventName, context);
            }

            public bool CanApply(object target)
            {
                return wrappedModifier.CanApply(target);
            }
        }
    }
}