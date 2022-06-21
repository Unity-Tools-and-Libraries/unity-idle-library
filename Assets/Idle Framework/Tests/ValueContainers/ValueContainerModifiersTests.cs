using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.Tests.ValueContainers.ValueContainerTest;

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
        public void CallingSetModifiersUnsubscribesTheExistingModifiers()
        {
            engine.CreateProperty("value", 1);
            var modifier = new AdditiveValueModifier("modifier", "", "value");
            engine.CreateProperty("path", modifiers: new List<IContainerModifier>()
            {
                modifier
            });
            Assert.AreEqual(1, modifier.CachedChangeListeners.Length);
            engine.GetProperty("path").SetModifiers(new List<IContainerModifier>());
            Assert.AreEqual(0, modifier.CachedChangeListeners.Length);
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
            engine.RegisterMethod("update", (e, ev) => BigDouble.One);
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

        /*
        public class ThrowOnCacheClearModifier : ValueModifier
        {
            public override object Apply(IdleEngine engine, ValueContainer container, object input)
            {
                throw new System.NotImplementedException();
            }
        }
        */
    }
}