using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Configuration;
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
            var reference = new ValueContainerDefinitionBuilder()
                .WithModifier(new AdditiveModifier("1", "1", 1))
                .Build().CreateValueReference(engine);
            Assert.AreEqual(new BigDouble(1), reference.ValueAsNumber());
        }

        [Test]
        public void AdditiveModifierAddedToSetValue()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithModifier(new AdditiveModifier("1", "1", 1))
                .Build().CreateValueReference(engine);
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void CanSpecifyAMultiplicativeModifierOnCreationWhichChangesNumberValue()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithModifier(new MultiplicativeModifier("1", "1", 2))
                .Build().CreateValueReference(engine);
            Assert.AreEqual(BigDouble.Zero, reference.ValueAsNumber());
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void ModifiersApplyToUpdateOutputValue()
        {
            var reference = new ValueContainerDefinitionBuilder()
                .WithUpdater((e, dt, v, p) =>
                {
                    return BigDouble.One;
                })
                .WithModifier(new AdditiveModifier("add", "", 1))
                .WithModifier(new MultiplicativeModifier("1", "1", 2))
                .Build().CreateValueReference(engine);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(4), reference.ValueAsNumber());
        }
    }
}