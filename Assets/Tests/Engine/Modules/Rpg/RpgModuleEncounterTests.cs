using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using MoonSharp.Interpreter;
using NUnit.Framework;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleEncounterTests : RpgModuleTestsBase
    {
        [Test]
        public void EncounterUpdateCallsUpdateOnCreatures()
        {
            random.SetNextValues(0);
            Configure();
            engine.Start();
            engine.StartEncounter();

            engine.SetActionPhase("combat");

            engine.Update(1f);

            Assert.AreEqual(new BigDouble(1), engine.GetPlayer<RpgCharacter>().ActionMeter);
            Assert.AreEqual(new BigDouble(1), engine.GetCurrentEncounter().Creatures[0].ActionMeter);
        }

        [Test]
        public void OnFinalEnemyKilledEndEncounter()
        {
            random.SetNextValues(0, 0);
            Configure();
            engine.Start();
            engine.StartEncounter();

            engine.SetActionPhase("combat");

            engine.Update(1f);

            bool called = false;

            engine.Watch(EncounterEndedEvent.EventName, "test", DynValue.FromObject(null, (Action)(() =>
            {
                called = true;
            })));

            engine.GetCurrentEncounter().Creatures[0].Kill();

            Assert.True(called);
        }

        [Test]
        public void OnEncounterEndScheduleCreactionOfNextEncounter()
        {
            random.SetNextValues(0, 0);
            Configure();
            engine.Start();
            engine.StartEncounter();

            engine.SetActionPhase("combat");

            engine.Update(1f);

            var currentEncounter = engine.GetCurrentEncounter();

            engine.GetCurrentEncounter().Creatures[0].Kill();

            var respawnTimer = engine.GetTimer(1);

            Assert.NotNull(respawnTimer);
            Assert.AreEqual("Timer to start new encounter.", respawnTimer.Description);
        }
    }
}