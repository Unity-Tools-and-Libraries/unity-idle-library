using BreakInfinity;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.Modifiers.CompositeModifier;
using static io.github.thisisnozaku.idle.framework.Modifiers.ContainerModifier;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class ExpressionEvaluationTests : RequiresEngineTests
    {
        [Test]
        public void SimpleStringEvaluatedToString()
        {
            Assert.AreEqual("string", engine.EvaluateExpression("\"string\""));
        }

        [Test]
        public void SimpleNumberEvaluatedToBigDouble()
        {
            Assert.AreEqual(new BigDouble(5), engine.EvaluateExpression("5"));
        }

        [Test]
        public void PathEvaluatesToContainer()
        {
            engine.CreateProperty("foo.bar", 5);
            Assert.AreEqual(typeof(ValueContainer), engine.EvaluateExpression("foo.bar").GetType());
        }

        [Test]
        public void CallingUserFunctionWrapsOutputInDynamicLuaValue()
        {
            engine.RegisterMethod("method", (a, b, c) =>
            {
                return 1;
            });
            Assert.DoesNotThrow(() =>
            {
                engine.EvaluateExpression("method()");
            });
        }

        [Test]
        public void FunctionInvocationCallsMethod()
        {
            int calls = 0;
            engine.RegisterMethod("method", (a, b, c) =>
            {
                calls++;
                return null;
            });
            engine.EvaluateExpression("method()");
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void FunctionInvocationPassesArguments()
        {
            int calls = 0;
            engine.RegisterMethod("method", (a, b, c) =>
            {
                calls++;
                Assert.AreEqual(new BigDouble(1), c[0]);
                Assert.AreEqual(new BigDouble(2), c[1]);
                Assert.AreEqual(new BigDouble(3), c[2]);
                return null;
            });
            engine.EvaluateExpression("method(1, 2, 3)");
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void CanUseCustomContext()
        {
            engine.CreateProperty("foo.bar", 5);
            var result = engine.EvaluateExpression("bar", new Dictionary<string, object>()
            {
                { "bar", engine.GetProperty("foo.bar") }
            });
            Assert.AreEqual(engine.GetProperty("foo.bar"), result);
        }
    }
}