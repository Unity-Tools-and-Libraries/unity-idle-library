using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.State {

    public class StateMachineCommandHandlerTests : TestsRequiringEngine
    {
        private StateMachine stateMachine;

        [SetUp]
        public void Setup()
        {
            stateMachine = new StateMachine(engine);
        }

        [Test]
        public void StateMachineThrowsIfCurrentStateCannotHandleCommand()
        {
            stateMachine.AddHandler(StateMachine.DEFAULT_STATE, "foo", (ie, cmd) => { }, "");
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.EvaluateCommand("foobar");
            });

            Assert.AreEqual(GenerateCommandErrorDescription("Current state 'initial' cannot handle command 'foobar'. ", "foo : " ), thrown.Message);
        }

        [Test]
        public void StateMachineHandlesTransitionCommand()
        {
            stateMachine.DefineTransition(StateMachine.DEFAULT_STATE, "destination", ie => null);
            Assert.DoesNotThrow(() =>
            {
                stateMachine.EvaluateCommand("transition destination");
            });
        }

        private string GenerateCommandErrorDescription(string error, params string[] otherCommands)
        {
            string message = error;
            foreach(var other in otherCommands)
            {
                message += Environment.NewLine + other;
            }
            return message;
        }
    }
}