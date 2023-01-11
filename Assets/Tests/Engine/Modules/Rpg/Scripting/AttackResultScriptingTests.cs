using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using MoonSharp.Interpreter;
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
        public void ScriptMustReturnAnAttackResultDescription()
        {
            rpgModule.Player.ToHitScript = "return 1";

            Configure();
            var attacker = new RpgCharacter(engine, 10);
            var defender = new RpgCharacter(engine, 11);
            engine.Scripting.EvaluateStringAsScript(engine.GetConfiguration<string>("player.Initializer"), Tuple.Create<string, object>("player", attacker)).ToObject<RpgCharacter>();
            engine.Scripting.EvaluateStringAsScript(engine.GetConfiguration<string>("creatures.Initializer"), new Dictionary<string, object>() { 
                { "creature", defender },
                { "definition", engine.GetCreatures()[1] },
                { "level", 1 }
            }).ToObject<RpgCharacter>();

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.MakeAttack(attacker, defender);
            });
        }
    }
}