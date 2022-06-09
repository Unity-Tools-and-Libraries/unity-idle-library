using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class MultiplicativeValueModifierTests : RequiresEngineTests
    {
        [Test]
        public void MultiplicativeModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
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
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new MultiplicativeValueModifier("", "", "foo", new string[] { "foo" }, contextGenerator: Context.GlobalContextGenerator)
            });
            engine.GetProperty("foo").Set(2);
            vc.Set(BigDouble.One);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
            engine.GetProperty("foo").Set(3);
            Assert.AreEqual(new BigDouble(3), vc.ValueAsNumber());
        }
    }
}