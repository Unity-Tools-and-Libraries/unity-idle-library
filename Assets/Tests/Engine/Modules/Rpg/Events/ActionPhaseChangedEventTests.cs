using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class ActionPhaseChangedEventTests
    {
        [Test]
        public void ContextVariablesContainsNewActionPhase()
        {
            var ev = new ActionPhaseChangedEvent("foo", "bar");
            Assert.AreEqual("foo", ev.GetContextVariables()[ActionPhaseChangedEvent.NewActionPhase]);
            Assert.AreEqual("bar", ev.GetContextVariables()[ActionPhaseChangedEvent.PreviousActionPhase]);
        }
    }
}