using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.Commands {
    public class RpgModuleKillCommandTests : RpgModuleTestsBase
    {
        [Test]
        public void KillCommandKillsPlayerAnyTime()
        {
            Configure();

            engine.EvaluateCommand("kill 1");
            Assert.IsFalse(engine.GetPlayer<RpgCharacter>().IsAlive);
        }

        [Test]
        public void KillCommandKillsCharacterInEncounter()
        {
            random.SetNextValues(0);
            Configure();
            engine.StartEncounter();

            engine.EvaluateCommand("kill 2");
            Assert.IsFalse(engine.GetCurrentEncounter().Creatures[0].IsAlive); 
        }
    }
}
