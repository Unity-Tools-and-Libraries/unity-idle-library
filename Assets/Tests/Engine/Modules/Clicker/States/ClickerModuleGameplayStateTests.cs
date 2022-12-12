using BreakInfinity;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker {
    public class ClickerModuleGameplayStateTests : ClickerModuleTestsBase
    {
        [SetUp]
        public void Setup()
        {
            Configure();
            engine.State.Transition("gameplay");
        }

        [Test]
        public void ClickAddsPoints()
        {
            engine.EvaluateCommand("click");

            Assert.AreEqual(BigDouble.One, engine.GetProperty<BigDouble>("player.Points.Quantity"));
        }
    }
}