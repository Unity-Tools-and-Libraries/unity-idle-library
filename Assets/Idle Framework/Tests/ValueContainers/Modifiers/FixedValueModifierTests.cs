using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Configuration;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers
{
    public class FixedValueModifierTests
    {
        private IdleEngine idleEngine;
        [SetUp]
        public void setup()
        {
            idleEngine = new IdleEngine(null, null);
        }

        [Test]
        public void FixedValueModifierSetsValue()
        {
            var vc = new ValueContainerDefinitionBuilder().WithModifier(new FixedValueModifier("1", "", BigDouble.One))
                .Build().CreateValueReference(idleEngine);
            vc.Set(BigDouble.Zero);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
        }

        [Test]
        public void FixedValueModifierTrumpsAllOtherModifiers()
        {
            var vc = new ValueContainerDefinitionBuilder().WithModifier(new FixedValueModifier("1", "", BigDouble.One))
                .WithModifier(new AdditiveValueModifier("2", "", 1000))
                .Build().CreateValueReference(idleEngine);
            vc.Set(BigDouble.Zero);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
        }
    }
}