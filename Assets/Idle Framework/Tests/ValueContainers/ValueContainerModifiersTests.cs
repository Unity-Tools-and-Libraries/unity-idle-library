using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers
{
    public class ValueContainerModifiersTests : RequiresEngineTests
    {
        [SetUp]
        public void setup()
        {
            InitializeEngine();
        }

        [Test]
        public void CanSpecifyAnAdditiveModifierOnCreationWhichAddsToNumberValue()
        {
            var reference = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>() {
                new AdditiveValueModifier("1", "1", 1)
            });
            reference.Set(0);
            Assert.AreEqual(new BigDouble(1), reference.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAddedToSetValue()
        {
            var reference = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>() {
                new AdditiveValueModifier("1", "1", 1)
            });
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void CanSpecifyAMultiplicativeModifierOnCreationWhichChangesNumberValue()
        {
            var reference = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>()
            {
                new MultiplicativeValueModifier("1", "1", 2)
            });
            Assert.AreEqual(BigDouble.Zero, reference.ValueAsNumber());
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void ModifiersApplyToUpdateOutputValue()
        {
            engine.RegisterMethod("update", (e, vc, ev) => BigDouble.One);
            var reference = engine.CreateProperty("path", BigDouble.Zero, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new AdditiveValueModifier("add", "", 1),
                new MultiplicativeValueModifier("1", "1", 2)
            }, "update");
            engine.Start();
            Assert.AreEqual(new BigDouble(1), reference.ValueAsNumber());
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(3), reference.ValueAsNumber());
        }
    }
}