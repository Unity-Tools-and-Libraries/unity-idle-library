using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class AttackResultScriptingTests : RpgModuleTestsBase
    {
        [Test]
        public void ScriptMustReturnAString()
        {
            rpgModule.Player.AttackToHitScript = "return 1";

            Configure();

            var attacker = new RpgCharacter(engine, 10);
            var defender = new RpgCharacter(engine, 11);
            
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.MakeAttack(attacker, defender);
            });
        }
    }
}