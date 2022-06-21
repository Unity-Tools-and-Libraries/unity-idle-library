using BreakInfinity;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine {
    public class IdleEngineScriptingTests: RequiresEngineTests
    {
        [Test]
        public void TryingToAddTwoNumberValueContainersCallsAddMetamethod()
        {
            engine.CreateProperty("a", 1);
            engine.CreateProperty("b", 2);
            Assert.AreEqual(new BigDouble(3), engine.EvaluateExpression("return a + b"));
        }

        [Test]
        public void TryingToAddNumberAndStringReturnsNanValueContainersCallsAddMetamethod()
        {
            engine.CreateProperty("a", 1);
            engine.CreateProperty("b", "foo");
            Assert.IsTrue(BigDouble.IsNaN((BigDouble)engine.EvaluateExpression("return a + b")));
        }

        [Test]
        public void TryingToAddNumberContainerAndStringReturnsNan()
        {
            engine.CreateProperty("a", 1);
            Assert.IsTrue(BigDouble.IsNaN((BigDouble)engine.EvaluateExpression("return a + 'foo'")));
        }

        [Test]
        public void TryingToAddNumberAndNumberStringReturnsNanValueContainersCallsAddMetamethod()
        {
            engine.CreateProperty("a", 1);
            engine.CreateProperty("b", "2");
            Assert.AreEqual(new BigDouble(3), engine.EvaluateExpression("return a + b"));
        }

        [Test]
        public void TryingToAddTwoStringValueContainersCallsAddMetamethod()
        {
            engine.CreateProperty("a", "foo");
            engine.CreateProperty("b", "bar");
            Assert.AreEqual("foobar", engine.EvaluateExpression("return a + b"));
        }

        [Test]
        public void TryingToAddStringToStringValueContainerCallsAddMetamethod()
        {
            engine.CreateProperty("b", "bar");
            Assert.AreEqual("foobar", engine.EvaluateExpression("return 'foo' + b"));
        }

        [Test]
        public void TryingToAddStringValueContainerToStringCallsAddMetamethod()
        {
            engine.CreateProperty("b", "bar");
            Assert.AreEqual("barfoo", engine.EvaluateExpression("return b + 'foo'"));
        }

        [Test]
        public void SubtractingNumbersSubtracts()
        {
            Assert.AreEqual(BigDouble.One, engine.EvaluateExpression("return 2 - 1"));
        }

        [Test]
        public void SubtractingNumberFromContainer()
        {
            engine.CreateProperty("a", 2);
            Assert.AreEqual(BigDouble.One, engine.EvaluateExpression("return a - 1"));
        }

        [Test]
        public void SubtractingContainerFromNumber()
        {
            engine.CreateProperty("a", 2);
            Assert.AreEqual(BigDouble.One, engine.EvaluateExpression("return 3 - a"));
        }

        [Test]
        public void CantSubtractStrings()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                Assert.AreEqual(BigDouble.One, engine.EvaluateExpression("return '3' - 'a'"));
            });
        }

        [Test]
        public void CantSubtractBools()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                Assert.AreEqual(BigDouble.One, engine.EvaluateExpression("return true - false"));
            });
        }

        [Test]
        public void CanDivideNumbers()
        {
            engine.CreateProperty("a", 10);
            engine.CreateProperty("b", 5);
            Assert.AreEqual(new BigDouble(2), engine.EvaluateExpression("return a / b"));
            Assert.AreEqual(new BigDouble(2), engine.EvaluateExpression("return a / 5"));
            Assert.AreEqual(new BigDouble(2), engine.EvaluateExpression("return 10 / b"));
            Assert.AreEqual(new BigDouble(2), engine.EvaluateExpression("return 10 / 5"));
        }

        [Test]
        public void CantDivideNonNumbers()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.EvaluateExpression("return true / 10");
            });
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.EvaluateExpression("return 10 / false");
            });
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.EvaluateExpression("return 'yes' / 10");
            });
        }

        [Test]
        public void CanNegateValue()
        {
            engine.CreateProperty("a", 1);            
            Assert.AreEqual(new BigDouble(-1), engine.EvaluateExpression("return -1"));
            Assert.AreEqual(new BigDouble(-1), engine.EvaluateExpression("return -a"));
        }

        [Test]
        public void CanCompareNumbers()
        {
            engine.CreateProperty("a", 1);
            engine.CreateProperty("b", 2);
            Assert.IsTrue((bool)engine.EvaluateExpression("return 1 == 1"));
            Assert.IsFalse((bool)engine.EvaluateExpression("return 1 == 2"));
            Assert.IsTrue((bool)engine.EvaluateExpression("return a == 1"));
            Assert.IsFalse((bool)engine.EvaluateExpression("return a == 2"));
            Assert.IsTrue((bool)engine.EvaluateExpression("return 1 == a"));
            Assert.IsFalse((bool)engine.EvaluateExpression("return 1 == b"));
            Assert.IsTrue((bool)engine.EvaluateExpression("return a == a"));
            Assert.IsFalse((bool)engine.EvaluateExpression("return a == b"));
        }
    }
}