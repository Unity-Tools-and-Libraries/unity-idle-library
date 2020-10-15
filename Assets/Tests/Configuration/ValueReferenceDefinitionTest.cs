using BreakInfinity;
using IdleFramework;
using IdleFramework.Configuration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Tests
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
            var definition = new ValueReferenceDefinitionBuilder().Build();
            var instantiated = definition.CreateValueReference(engine);
            Assert.AreEqual(BigDouble.Zero, instantiated.ValueAsNumber());
            Assert.AreEqual(false, instantiated.ValueAsBool());
            Assert.AreEqual(null, instantiated.ValueAsMap());
            Assert.AreEqual("0", instantiated.ValueAsString());
        }

        [Test]
        public void CanDefineAnUpdaterAction()
        {
            var definition = new ValueReferenceDefinitionBuilder()
                .WithUpdater((engine, parent, deltaTime, previousValue) => {
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
            ValueReferenceDefinition def = "foobar";
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanImplicitlyConvertFromBigDoubleToDefinition()
        {
            ValueReferenceDefinition def = BigDouble.One;
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanImplicitlyConvertFromBooleanToDefinition()
        {
            ValueReferenceDefinition def = true;
            Assert.IsNotNull(def);
        }

        [Test]
        public void CanImplicitlyConvertFromDictionaryToDefinition()
        {
            ValueReferenceDefinition def = new Dictionary<string, ValueReferenceDefinition>();
            Assert.IsNotNull(def);
        }
    }
}