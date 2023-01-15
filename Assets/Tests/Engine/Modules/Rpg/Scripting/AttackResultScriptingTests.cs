using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration;
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
            rpgModule.Player.ToHitScript = DynValue.NewString("return 1");

            Configure();
            var attacker = new RpgCharacter(engine, 10);
            var defender = new RpgCharacter(engine, 11);
            engine.Scripting.Evaluate(engine.GetConfiguration<PlayerConfiguration>("player").Initializer, Tuple.Create<string, object>("player", attacker),
                new List<string>() { "player" }).ToObject<RpgCharacter>();
            engine.Scripting.Evaluate(engine.GetExpectedConfiguration<CreaturesConfiguration>("creatures").Initializer,
                new Dictionary<string, object>() { 
                    { "creature", defender },
                    { "definition", engine.GetCreatures()[1] },
                    { "level", BigDouble.One }
                }, new List<string>() { "creature", "definition", "level" }).ToObject<RpgCharacter>();

            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.MakeAttack(attacker, defender);
            });
        }
    }
}