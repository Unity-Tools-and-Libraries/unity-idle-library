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
            Assert.Throws(typeof(ArgumentNullException), () => {
                new AttackHitEvent(null, null, new BigDouble(1));
            });
            Assert.Throws(typeof(ArgumentNullException), () => {
                new AttackHitEvent(new RpgCharacter(engine), null, new BigDouble(1));
            });
            Assert.DoesNotThrow(() => {
                new AttackHitEvent(new RpgCharacter(engine), new RpgCharacter(engine), new BigDouble(1));
            });
        }
    }
}