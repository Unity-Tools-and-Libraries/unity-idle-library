using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using io.github.thisisnozaku.idle.framework.Engine.State;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.State
{
    public class StateMachineEventsTest : TestsRequiringEngine
    {
        private StateMachine stateMachine;

        [SetUp]
        public void setup()
        {
            stateMachine = new StateMachine(engine);
        }

        [Test]
        public void EmitsEventOnStateChange()
        {
            bool called = false;
            string oldState = null;
            string newState = null;
            engine.Scripting.Globals["callback"] = (Action<StateChangedEvent>)((StateChangedEvent ev) =>
            {
                called = true;
                oldState = ev.PreviousState;
                newState = ev.NewState;
            });

            stateMachine.Watch(StateChangedEvent.EventName, "test", "callback(ev)");

            stateMachine.DefineTransition("initial", "destination");

            stateMachine.Transition("destination");

            Assert.IsTrue(called);
            Assert.AreEqual("initial", oldState);
            Assert.AreEqual("destination", newState);
        }
    }
}