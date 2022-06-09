using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class ValueContainerEventsTests : RequiresEngineTests
    {
        [Test]
        public void WatchingForChangesReceivesCurrentValueImmediately()
        {
            var valueReference = engine.CreateProperty("path");
            bool called = false;
            engine.RegisterMethod("method", (ie, c, ev) => { called = true; return null; });
            engine.Start();
            valueReference.Subscribe("", ValueChangedEvent.EventName, "method");
            Assert.IsTrue(called);
        }

        [Test]
        public void ChangingValueOfWatchedValueNotifiesListeners()
        {
            var valueReference = engine.CreateProperty("path");
            int calls = 0;
            engine.RegisterMethod("method", (ie, c, ev) =>
            {
                var newValue = ev[2];
                calls++;
                if (calls == 1)
                {
                    Assert.Null(newValue);
                }
                else
                {
                    Assert.AreEqual(BigDouble.One, newValue);
                }
                return null;
            });
            engine.Start();
            valueReference.Subscribe("", ValueChangedEvent.EventName, "method");
            valueReference.Set(BigDouble.One);
            Assert.AreEqual(2, calls);
        }

        //[Test]
        public void ErrorInEventListenerLogsMessage()
        {
            LogAssert.Expect(LogType.Error, "Error trying to invoke listener from \"event\" for event \"event\": System.Exception: Exception of type 'System.Exception' was thrown.\n" +
                "  at io.github.thisisnozaku.idle.framework.Tests.ValueContainers.ValueContainerEventsTests+<>c.<ErrorInEventListenerLogsMessage>b__2_0 (io.github.thisisnozaku.idle.framework.IdleEngine ie, io.github.thisisnozaku.idle.framework.ValueContainer vc, System.Object[] ev) [0x00001] in D:\\Unity Projects\\IdleFramework\\Assets\\Idle Framework\\Tests\\ValueContainers\\Events\\ValueContainerEventsTests.cs:60 \n" +
                "  at io.github.thisisnozaku.idle.framework.IdleEngine.InvokeMethod (System.String methodName, io.github.thisisnozaku.idle.framework.ValueContainer container, System.Object arg) [0x00014] in D:\\Unity Projects\\IdleFramework\\Assets\\Idle Framework\\Runtime\\Scripts\\IdleEngine.cs:399 \n" +
                "  at io.github.thisisnozaku.idle.framework.ValueContainer.DoListenerInvocation (io.github.thisisnozaku.idle.framework.ValueContainer+ListenerSubscription listener, io.github.thisisnozaku.idle.framework.Events.IdleEngineEvent ev, System.String eventName) [0x00044] in D:\\Unity Projects\\IdleFramework\\Assets\\Idle Framework\\Runtime\\Scripts\\ValueContainer.cs:555 ."
                );

            engine.RegisterMethod("method", (ie, vc, ev) => {
                throw new System.Exception();
            });
            engine.CreateProperty("path").Subscribe("event", "event", "method");
            engine.Start();
            engine.GetProperty("path").NotifyImmediately("event", null);
        }
    }
}