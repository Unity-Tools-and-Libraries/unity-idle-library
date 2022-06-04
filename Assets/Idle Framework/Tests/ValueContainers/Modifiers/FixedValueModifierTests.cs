using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
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
            var vc = engine.SetProperty("path", BigDouble.Zero, "", new List<ContainerModifier>() {
                new SetValueModifier("1", "", BigDouble.One)
            });
            vc.Set(BigDouble.Zero);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
        }

        [Test]
        public void FixedValueModifierTrumpsAllOtherModifiers()
        {
            var vc = engine.SetProperty("path", BigDouble.One, "", new List<ContainerModifier>() {
                new SetValueModifier("1", "", BigDouble.One)
            });
            vc.Set(BigDouble.Zero);
            Assert.AreEqual(BigDouble.One, vc.ValueAsNumber());
        }

        [Test]
        public void FixedValueModifierCanSetABool() {
            var vc = engine.SetProperty("path", false, "", new List<ContainerModifier>() {
                new SetValueModifier("1", "", true)
            });
            Assert.IsTrue(vc.ValueAsBool());
        }
    }
}