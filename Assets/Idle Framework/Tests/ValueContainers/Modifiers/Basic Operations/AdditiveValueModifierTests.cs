using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;
using static io.github.thisisnozaku.idle.framework.Tests.ValueContainers.ValueContainerTest;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class AdditiveValueModifierTests : RequiresEngineTests
    {
        
        [Test]
        public void AdditiveModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
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
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new AdditiveValueModifier("", "", "value", new string[] { "value" }, Context.GlobalContextGenerator)
            });
            engine.CreateProperty("value", 1);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            engine.CreateProperty("value", 2);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAppliesAfterMultiplication()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>()
            {
                new AdditiveValueModifier("2", "", 1),
                new MultiplicativeValueModifier("1", "", 2)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }

        [Test]
        public void WhenAdditiveModifierIsCalculatedClearCachedModifierValueWhenUnderlyingValueChanged()
        {
            engine.ConfigureLogging("engine.internal.modifier", UnityEngine.LogType.Log);
            engine.Start();
            var vc = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>()
            {
                new CallCountingModifier(new AdditiveValueModifier("2", "", "value", new string[] { "value" }))
            });
            engine.GetProperty("value").Set(1);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
            engine.GetProperty("value").Set(2);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
            engine.Update(1);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
        }
    }
}