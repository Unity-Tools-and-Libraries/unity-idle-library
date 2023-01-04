using io.github.thisisnozaku.idle.framework.Engine.State;
using NUnit.Framework;
using System;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.State
{
    public class StateMachineTests : TestsRequiringEngine
    {
        private StateMachine stateMachine;

        [SetUp]
        public void Setup()
        {
            stateMachine = new StateMachine(engine);
        }

        [Test]
        public void StateMachineCanDefineInitialState()
        {
            Assert.AreEqual(StateMachine.DEFAULT_STATE, stateMachine.StateName);
        }

        [Test]
        public void StateMachineCanTransitionToNamedState()
        {
            stateMachine.DefineTransition(StateMachine.DEFAULT_STATE, "destination");
            Assert.DoesNotThrow(() =>
            {
                stateMachine.Transition("destination");
            });
            Assert.AreEqual("destination", stateMachine.StateName);
        }

        [Test]
        public void StateMachineThrowsTransitioningToUnknownState()
        {
            stateMachine.DefineTransition(StateMachine.DEFAULT_STATE, "destination");
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.Transition("foobar");
            });
            Assert.AreEqual("No transition defined from 'initial' -> 'foobar'.", thrown.Message);
        }

        [Test]
        public void StateMachineThrowsTransitioningFromUnknownState()
        {
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.Transition("foobar");
            });
            Assert.AreEqual("No transitions are defined from state 'initial'.", thrown.Message);
        }

        [Test]
        public void StateMachineCanDefineTransitionGuard()
        {
            stateMachine.DefineTransition("initial", "destination", ie => "foobar");
            var thrown = Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.Transition("destination");
            });
            Assert.AreEqual("Blocked transition from 'initial' -> 'destination': foobar.", thrown.Message);
        }
    }
}