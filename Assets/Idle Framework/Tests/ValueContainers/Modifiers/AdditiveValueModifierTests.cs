using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers
{
    public class AdditiveValueModifierTests : RequiresEngineTests
    {
        
        [Test]
        public void AdditiveModifierAddsToSetValue()
        {
            var vc = engine.CreateValueContainer(null as string, new System.Collections.Generic.List<ValueModifier>()
            {
                new AdditiveValueModifier("", "", 1)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            vc.Set(5);
            Assert.AreEqual(new BigDouble(6), vc.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAppliesAfterMultiplication()
        {
            var vc = engine.CreateValueContainer(modifiers: new System.Collections.Generic.List<ValueModifier>()
            {
                new AdditiveValueModifier("2", "", 1),
                new MultiplicativeValueModifier("1", "", 2)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }
    }
}