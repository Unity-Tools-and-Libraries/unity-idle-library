using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers
{
    public class FixedValueModifierTests : RequiresEngineTests
    {
        [Test]
        public void FixedValueModifierSetsValue()
        {
            var vc = engine.CreateValueContainer(BigDouble.Zero, new List<ValueModifier>() {
                new FixedValueModifier("1", "", BigDouble.One)
            });
            vc.Set(BigDouble.Zero);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
        }

        [Test]
        public void FixedValueModifierTrumpsAllOtherModifiers()
        {
            var vc = engine.CreateValueContainer(BigDouble.One, new List<ValueModifier>() {
                new FixedValueModifier("1", "", BigDouble.One)
            });
            vc.Set(BigDouble.Zero);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
        }
    }
}