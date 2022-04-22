using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueReferenceDefinitionTest
    {
        IdleEngine engine;
        [SetUp]
        public void Setup() {
            engine = new IdleEngine(null, null);
        }
        [Test]
        public void InstatiatesWithDefaultValueIfNonDefined()
        {
            var definition = new ValueContainerDefinitionBuilder().Build();
            var instantiated = definition.CreateValueReference(engine);
            Assert.AreEqual(BigDouble.Zero, instantiated.ValueAsNumber());
            Assert.AreEqual(false, instantiated.ValueAsBool());
            Assert.AreEqual(null, instantiated.ValueAsMap());
            Assert.AreEqual("0", instantiated.ValueAsString());
        }

        [Test]
        public void CanDefineAnUpdaterAction()
        {
            var definition = new ValueContainerDefinitionBuilder()
                .WithUpdater((engine, deltaTime, previousValue, parent) => {
                    return (BigDouble)previousValue + BigDouble.One;
                })
                .Build();
            var instantiated = definition.CreateValueReference(engine);
            Assert.AreEqual(BigDouble.Zero, instantiated.ValueAsNumber());
            instantiated.Update(engine, 1f);
            Assert.AreEqual(BigDouble.One, instantiated.ValueAsNumber());
        }

        [Test]
        public void CanImplicitlyConvertFromStringToDefinition()
        {
            ValueContainerDefinition def = "foobar";
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanImplicitlyConvertFromBigDoubleToDefinition()
        {
            ValueContainerDefinition def = BigDouble.One;
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanImplicitlyConvertFromBooleanToDefinition()
        {
            ValueContainerDefinition def = true;
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanImplicitlyConvertFromDictionaryToDefinition()
        {
            ValueContainerDefinition def = new Dictionary<string, ValueContainerDefinition>();
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanDefineAPostUpdateHook()
        {
            bool hookCalled = false;
            Action<IdleEngine, float, object> hook = (engine, deltaTime, newValue) =>
            {
                hookCalled = true;
            };
            ValueContainer reference = new ValueContainerDefinitionBuilder()
                .WithPostUpdateHook(hook)
                .Build().CreateValueReference(engine);
            reference.Update(engine, 0);

            Assert.IsTrue(hookCalled);
        }
    }
}