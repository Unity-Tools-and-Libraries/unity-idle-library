using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System.Collections.Generic;

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
                new ValueModifier("1", "1", "return value + 1", null, "return value != nil")
            });
            reference.Set(0);
            Assert.AreEqual(new BigDouble(1), reference.ValueAsNumber());
        }

        //[Test]
        public void CallingSetModifiersUnsubscribesTheExistingModifiers()
        {
            engine.CreateProperty("globalValue", 1);
            var modifier = new ValueModifier("", "", "return value + globalValue", engine);
            engine.CreateProperty("path", modifiers: new List<IContainerModifier>()
            {
                modifier
            });
            //Assert.AreEqual(1, modifier.CachedChangeListeners.Length);
            engine.GetProperty("path").SetModifiers(new List<IContainerModifier>());
            //Assert.AreEqual(0, modifier.CachedChangeListeners.Length);
        }

        [Test]
        public void AdditiveModifierAddedToSetValue()
        {
            var reference = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>() {
                new ValueModifier("1", "1", "return value + 1", null, "return value != nil")
            });
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void CanSpecifyAMultiplicativeModifierOnCreationWhichChangesNumberValue()
        {
            var reference = engine.CreateProperty("path", modifiers: new System.Collections.Generic.List<IContainerModifier>()
            {
                new ValueModifier("1", "1", "return value * 2", null, "return value != nil")
            });
            Assert.AreEqual(BigDouble.Zero, reference.ValueAsNumber());
            reference.Set(1);
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
        }

        [Test]
        public void ModifiersApplyToUpdateOutputValue()
        {
            engine.RegisterMethod("update", (e, ev) => DynValue.FromObject(e.GetScript(), BigDouble.One));
            var reference = engine.CreateProperty("path", BigDouble.Zero, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new ValueModifier("add", "", "return value + 1", null, "return value != nil", ValueModifier.DefaultPriorities.ADDITION),
                new ValueModifier("add", "", "return value * 2", null, "return value != nil", ValueModifier.DefaultPriorities.MULTIPLICATION),
            }, "return update()");
            engine.Start();
            Assert.AreEqual(new BigDouble(2), reference.ValueAsNumber());
            engine.Update(1f);
            Assert.AreEqual(new BigDouble(4), reference.ValueAsNumber());
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