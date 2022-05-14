using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests
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
            var reference = engine.CreateValueContainer(modifiers: new System.Collections.Generic.List<ValueModifier>() {
                new AdditiveValueModifier("1", "1", 1)
            });
            Assert.AreEqual(new BigDouble(1), reference.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAddedToSetValue()
        {
            var reference = engine.CreateValueContainer(modifiers: new System.Collections.Generic.List<ValueModifier>() {
                new AdditiveValueModifier("1", "1", 1)
            });
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void CanSpecifyAMultiplicativeModifierOnCreationWhichChangesNumberValue()
        {
            var reference = engine.CreateValueContainer(modifiers: new System.Collections.Generic.List<ValueModifier>()
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
            var reference = engine.CreateValueContainer(BigDouble.Zero, "", new System.Collections.Generic.List<ValueModifier>()
            {
                new AdditiveValueModifier("add", "", 1),
                new MultiplicativeValueModifier("1", "1", 2)
            }, (e, dt, v, c, mds) =>
            {
                return BigDouble.One;
            });
            Assert.AreEqual(new BigDouble(1), reference.ValueAsNumber());
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(3), reference.ValueAsNumber());
        }
    }
}