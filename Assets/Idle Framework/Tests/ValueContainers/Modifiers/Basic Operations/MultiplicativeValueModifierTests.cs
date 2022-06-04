using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class MultiplicativeValueModifierTests : RequiresEngineTests
    {
        [Test]
        public void MultiplicativeModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.SetProperty("path", null as string, "", new System.Collections.Generic.List<ContainerModifier>()
            {
                new MultiplicativeValueModifier("", "", 2)
            });
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            vc.Set(5);
            Assert.AreEqual(new BigDouble(10), vc.ValueAsNumber());
        }

        [Test]
        public void MultiplicativeModifierCanHaveADynamicValue()
        {
            engine.Start();
            var vc = engine.SetProperty("path", null as string, "", new System.Collections.Generic.List<ContainerModifier>()
            {
                new MultiplicativeValueModifier("", "", "foo")
            });
            engine.SetProperty("foo", 2);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            engine.SetProperty("foo", 3);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }
    }
}