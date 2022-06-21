using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class SubtractiveValueModifierTests : RequiresEngineTests
    {

        [Test]
        public void SubtractiveModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
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
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new SubtractiveValueModifier("", "", "value", new string[] { "value" }, contextGenerator: Context.GlobalContextGenerator)
            });
            engine.GetProperty("value").Set(1);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(0), vc.ValueAsNumber());
            engine.GetProperty("value").Set(2);
            Assert.AreEqual(new BigDouble(-1), vc.ValueAsNumber());
        }

        [Test]
        public void SubtractiveModifierAppliesAfterMultiplication()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>()
            {
                new SubtractiveValueModifier("2", "", 2),
                new MultiplicativeValueModifier("1", "", 2)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(0), vc.ValueAsNumber());
        }
    }
}