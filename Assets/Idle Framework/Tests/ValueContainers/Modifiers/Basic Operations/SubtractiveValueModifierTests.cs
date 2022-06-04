using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class SubtractiveValueModifierTests : RequiresEngineTests
    {

        [Test]
        public void SubtractiveModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.SetProperty("path", null as string, "", new System.Collections.Generic.List<ContainerModifier>()
            {
                new SubtractiveValueModifier("", "", 1)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(0), vc.ValueAsNumber());
            vc.Set(5);
            Assert.AreEqual(new BigDouble(4), vc.ValueAsNumber());
        }

        [Test]
        public void SubtractiveModifierSubtractsDynamicValue()
        {
            engine.Start();
            var vc = engine.SetProperty("path", null as string, "", new System.Collections.Generic.List<ContainerModifier>()
            {
                new SubtractiveValueModifier("", "", "value")
            });
            engine.SetProperty("value", 1);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(0), vc.ValueAsNumber());
            engine.SetProperty("value", 2);
            Assert.AreEqual(new BigDouble(-1), vc.ValueAsNumber());
        }

        [Test]
        public void SubtractiveModifierAppliesAfterMultiplication()
        {
            engine.Start();
            var vc = engine.SetProperty("path", modifiers: new System.Collections.Generic.List<ContainerModifier>()
            {
                new SubtractiveValueModifier("2", "", 2),
                new MultiplicativeValueModifier("1", "", 2)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(0), vc.ValueAsNumber());
        }
    }
}