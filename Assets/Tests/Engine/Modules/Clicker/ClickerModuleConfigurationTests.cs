using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using MoonSharp.Interpreter;
using NUnit.Framework;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModuleConfigurationTests : TestsRequiringEngine
    {
        [SetUp]
        public void setup()
        {
            engine.AddModule(new ClickerModule());
        }

        [Test]
        public void SetsPointsProperties()
        {
            Assert.AreNotEqual(DataType.Nil, engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points')").Type);
            Assert.AreEqual(new BigDouble(0), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').quantity").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(0), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').totalIncome").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').click_income").ToObject<BigDouble>());
        }

        [Test]
        public void SetsPlayerProducersProperties()
        {
            Assert.NotNull(engine.Scripting.EvaluateStringAsScript("return globals.player.producers").ToObject());
        }

        [Test]
        public void DoClickIncreasesPointsByClickIncomeValue()
        {
            Assert.AreEqual(new BigDouble(0), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').quantity").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').click_income").ToObject<BigDouble>());
            engine.Scripting.EvaluateStringAsScript("engine.DoClick('points')");
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').quantity").ToObject<BigDouble>());
        }

        [Test]
        public void ClickIncomeIsEqualToBaseTimesMultiplier()
        {
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').click_income_base").ToObject<BigDouble>());
            Assert.AreEqual(new BigDouble(1), engine.Scripting.EvaluateStringAsScript("return globals.player.GetResource('points').click_income_multiplier").ToObject<BigDouble>());

        }
    }
}