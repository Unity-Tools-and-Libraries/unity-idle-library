using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class EncounterEventsTests : MonoBehaviour
    {
        [Test]
        public void EndEventHasNoContext()
        {
            Assert.IsNull(new EncounterEndedEvent().GetContextVariables());
        }

        [Test]
        public void StartEventHasNoContext()
        {
            Assert.IsNull(new EncounterStartedEvent().GetContextVariables());
        }
    }
}