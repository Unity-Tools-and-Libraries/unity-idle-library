using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleEvents : RpgModuleTestsBase
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
    }
}