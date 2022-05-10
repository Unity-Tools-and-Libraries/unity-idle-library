using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueContainerFunctionTests : RequiresEngineTests
    {
        [Test]
        public void FunctionReceivesIdleEngineArgument()
        {
            var reference = engine.CreateValueContainer((ie, c, val) =>
            {
                Assert.AreEqual(ie, engine);
                return null;
            });
            reference.ValueAsFunction().Invoke("argument");
        }

        [Test]
        public void FunctionReceivesContainer()
        {
            ValueContainer reference = null;
            reference = engine.CreateValueContainer((ie, c, val) =>
            {
                Assert.AreEqual(c, reference);
                Assert.NotNull(c);
                return null;
            });
            reference.ValueAsFunction().Invoke("argument");
        }

        [Test]
        public void FunctionReceivesArgumentArray()
        {
            ValueContainer reference = null;
            reference = engine.CreateValueContainer((ie, c, val) =>
            {
                Assert.AreEqual(val[0], true);
                Assert.AreEqual(val[1], "one");
                Assert.AreEqual(val[2], new BigDouble(2));
                return null;
            });
            reference.ValueAsFunction().Invoke(true, "one", 2);
        }
    }
}