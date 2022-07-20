using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
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
            
            Assert.Throws(typeof(ArgumentNullException), () => {
                new AttackHitEvent(null, null, new BigDouble(1));
            });
            Assert.Throws(typeof(ArgumentNullException), () => {
                new AttackHitEvent(attacker, null, new BigDouble(1));
            });
            Assert.DoesNotThrow(() => {
                new AttackHitEvent(attacker, defender, new BigDouble(1));
            });
        }

        [Test]
        public void InflictingDamageToCharacterEmitsOnSource()
        {
            Configure();

            var source = new RpgCharacter(engine, 100);
            var target = new RpgCharacter(engine, 101);

            source.Watch(DamageInflictedEvent.EventName, "test", "triggered = true");

            target.InflictDamage(new BigDouble(1), source);

            Assert.IsTrue((bool)engine.GlobalProperties["triggered"]);
        }
    }
}