using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Configuration;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers
{
    public class AdditiveValueModifierTests
    {
        private IdleEngine idleEngine;
        [SetUp]
        public void setup()
        {
            idleEngine = new IdleEngine(null, null);
        }

        [Test]
        public void AdditiveModifierAddsToSetValue()
        {
            var vc = new ValueContainerDefinitionBuilder()
                .WithModifier(new AdditiveValueModifier("", "", 1))
                .Build().CreateValueReference(idleEngine);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            vc.Set(5);
            Assert.AreEqual(new BigDouble(6), vc.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAppliesAfterMultiplication()
        {
            var vc = new ValueContainerDefinitionBuilder()
                .WithModifier(new AdditiveValueModifier("2", "", 1))
                .WithModifier(new MultiplicativeValueModifier("1", "", 2))
                .Build().CreateValueReference(idleEngine);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }
    }
}