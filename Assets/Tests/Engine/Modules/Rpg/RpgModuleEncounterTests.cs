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

            engine.Update(1f);

            engine.GetCurrentEncounter().Creatures[0].Kill();

            var respawnTimer = engine.GetTimer(1);

            Assert.NotNull(respawnTimer);
            Assert.AreEqual("Timer to start new encounter.", respawnTimer.Description);
            Assert.AreEqual(0.5, respawnTimer.Duration);
        }

        [Test]
        public void LevelOfCreaturesInEncounterBasedOnEncounterLevel()
        {
            Configure();
            random.SetNextValues(0, 0);

            engine.StartEncounter();

            Assert.AreEqual(BigDouble.One, engine.GetCurrentEncounter().Level);
            Assert.AreEqual(BigDouble.One, engine.GetCurrentEncounter().Creatures[0].Level);

            engine.SetStage(new BigDouble(2));

            Assert.AreEqual(new BigDouble(2), engine.GetCurrentEncounter().Level);
            Assert.AreEqual(new BigDouble(2), engine.GetCurrentEncounter().Creatures[0].Level);
        }

        [Test]
        public void ChangingStageEndsEncounter()
        {
            Configure();
            random.SetNextValues(0, 0);

            var encounter = engine.GetCurrentEncounter();

            engine.SetStage(new BigDouble(1));

            Assert.AreNotEqual(encounter, engine.GetCurrentEncounter());
        }

        [Test]
        public void WhenPlayerDiesEncounterEnds()
        {
            random.SetNextValues(0, 0);
            Configure();
            engine.Start();
            engine.StartEncounter();

            Assert.True(engine.GetCurrentEncounter().IsActive);

            engine.GetPlayer<RpgCharacter>().Kill();

            Assert.False(engine.GetCurrentEncounter().IsActive);
        }
    }
}