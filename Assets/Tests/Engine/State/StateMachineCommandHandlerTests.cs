using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.State {

    public class StateMachineCommandHandlerTests : TestsRequiringEngine
    {
        private StateMachine stateMachine;

        [SetUp]
        public void Setup()
        {
            stateMachine = new StateMachine();
        }

        [Test]
        public void StateMachineThrowsIfCurrentStateCannotHandleCommand()
        {
            stateMachine.AddHandler(StateMachine.DEFAULT_STATE, "", (ie, cmd) => { }, "");
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.EvaluateCommand("foobar", engine);
            });

            Assert.AreEqual("Current state 'initial' cannot handle command 'foobar'", thrown.Message);
        }

        [Test]
        public void StateMachineHandlesTransitionCommand()
        {
            stateMachine.DefineTransition(StateMachine.DEFAULT_STATE, "destination", ie => null);
            Assert.DoesNotThrow(() =>
            {
                stateMachine.EvaluateCommand("transition destination", engine);
            });
        }
    }
}