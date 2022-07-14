using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Rpg
{
    public class RpgModuleCombatTests : RpgModuleTestsBase
    {
        [Test]
        public void AttackRollGreaterThanHitChanceHits()
        {
            Configure();

            random.SetNextValues(1, 1, 500);

            engine.StartEncounter();

            var result = engine.MakeAttack(engine.GetPlayer(), engine.GetCurrentEncounter().Creatures[0]);

            Assert.AreEqual("hit", result.Description);
        }
    }
}