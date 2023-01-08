using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg.Commands
{
    public class RpgModuleDamageCommandTests : RpgModuleTestsBase
    {
        [Test]
        public void DamageCommandReducesHealthOfTarget()
        {
            Configure();

            engine.EvaluateCommand("damage 1 1");
            Assert.AreEqual(engine.GetPlayer<RpgCharacter>().MaximumHealth.Total - 1,
                engine.GetPlayer<RpgCharacter>().CurrentHealth);
        }
    }
}
