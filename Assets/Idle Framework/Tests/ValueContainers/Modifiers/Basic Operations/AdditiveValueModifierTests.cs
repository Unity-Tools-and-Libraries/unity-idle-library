using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class AdditiveValueModifierTests : RequiresEngineTests
    {
        
        [Test]
        public void AdditiveModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.SetProperty("path", null as string, "", new System.Collections.Generic.List<ContainerModifier>()
            {
                new AdditiveValueModifier("", "", 1)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            vc.Set(5);
            Assert.AreEqual(new BigDouble(6), vc.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAddsDynamicValue()
        {
            engine.Start();
            var vc = engine.SetProperty("path", null as string, "", new System.Collections.Generic.List<ContainerModifier>()
            {
                new AdditiveValueModifier("", "", "value")
            });
            engine.SetProperty("value", 1);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            engine.SetProperty("value", 2);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAppliesAfterMultiplication()
        {
            engine.Start();
            var vc = engine.SetProperty("path", modifiers: new System.Collections.Generic.List<ContainerModifier>()
            {
                new AdditiveValueModifier("2", "", 1),
                new MultiplicativeValueModifier("1", "", 2)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }
    }
}