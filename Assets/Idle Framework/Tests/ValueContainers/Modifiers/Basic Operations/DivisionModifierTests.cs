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
    public class DivisionModifierTests: RequiresEngineTests
    {
        [Test]
        public void MultiplicativeModifierAddsToSetValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new DivisionValueModifier("", "", 2)
            });
            vc.Set(10);
            Assert.AreEqual(new BigDouble(5), vc.ValueAsNumber());
        }

        [Test]
        public void DivisionModifierCanHaveDynamicValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", null as string, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new DivisionValueModifier("", "", "foo.bar", new string[] { "foo.bar" }, contextGenerator: Context.GlobalContextGenerator)
            });
            engine.GetProperty("foo.bar").Set(2);
            vc.Set(10);
            Assert.AreEqual(new BigDouble(5), vc.ValueAsNumber());
            engine.CreateProperty("foo.bar", 5);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
        }
    }
}