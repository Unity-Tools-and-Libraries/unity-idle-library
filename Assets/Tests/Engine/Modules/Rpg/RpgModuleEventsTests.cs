using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleEventsTests : RpgModuleTestsBase
    {
        [Test]
        public void AttackHitEventNeedsAttackerAndDefender()
        {
            Configure();

            var attacker = new RpgCharacter(engine, engine.GetNextAvailableId());
            var defender = new RpgCharacter(engine, engine.GetNextAvailableId());

            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                new AttackHitEvent(null, null, null);
            });
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                new AttackHitEvent(null, null, null);
            });
            Assert.DoesNotThrow(() =>
            {
                new AttackHitEvent(attacker, defender, new AttackResultDescription(true, "", BigDouble.Zero, attacker,
                    new List<long>(), new List<long>()));
            });
        }

        [Test]
        public void InflictingDamageToCharacterEmitsOnSource()
        {
            random.SetNextValues(0);
            Configure();

            var source = new RpgCharacter(engine, 100);
            var target = new RpgCharacter(engine, 101);

            source.Watch(DamageInflictedEvent.EventName, "test", "globals.triggered = true");

            target.InflictDamage(new BigDouble(1), source);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void EventEmittedWhenCharacterActs()
        {
            Configure();

            engine.GetPlayer<RpgCharacter>().Watch(CharacterActedEvent.EventName, "test", DynValue.FromObject(null, (Action<IDictionary<string, object>>)(ctx =>
            {
                Assert.AreEqual("foo", ctx["action"]);
            })));
            engine.GetPlayer<RpgCharacter>().Emit(CharacterActedEvent.EventName, new CharacterActedEvent(engine.GetPlayer<RpgCharacter>(), "foo"));
        }

        [Test]
        public void ChangingStageEmitsEvent()
        {
            Configure();

            random.SetNextValues(0);

            engine.Watch(StageChangedEvent.EventName, "test", "globals.triggered = true");

            engine.SetStage(1);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }

        [Test]
        public void ChangingActionPhaseEmitsEvent()
        {

        }
    }
}