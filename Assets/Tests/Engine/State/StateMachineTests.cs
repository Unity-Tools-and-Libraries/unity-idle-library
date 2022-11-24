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
            stateMachine = new StateMachine();
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
                stateMachine.Transition("destination", engine);
            });
            Assert.AreEqual("destination", stateMachine.StateName);
        }

        [Test]
        public void StateMachineThrowsTransitioningToUnknownState()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.Transition("foobar", engine);
            });
        }

        [Test]
        public void StateMachineThrowsTransitioningFromUnknownState()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.Transition("foobar", engine);
            });
        }

        [Test]
        public void StateMachineCanDefineTransitionGuard()
        {
            stateMachine.DefineTransition("origin", "destination", ie => "foobar");
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                stateMachine.Transition("destination", engine);
            });
        }
    }
}