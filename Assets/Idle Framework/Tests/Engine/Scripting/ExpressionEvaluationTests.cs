using BreakInfinity;
using MoonSharp.Interpreter;
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
            Assert.AreEqual("string", engine.EvaluateExpression("return \"string\""));
        }

        [Test]
        public void SimpleNumberEvaluatedToBigDouble()
        {
            Assert.AreEqual(new BigDouble(5), engine.EvaluateExpression("return 5"));
        }

        [Test]
        public void PathEvaluatesToContainer()
        {
            engine.CreateProperty("foo.bar", 5);
            Assert.AreEqual(typeof(ValueContainer), engine.EvaluateExpression("return foo.bar", "global").GetType());
        }

        [Test]
        public void CallingUserFunctionWrapsOutputInDynamicLuaValue()
        {
            engine.RegisterMethod("method", (a, c) =>
            {
                return DynValue.FromObject(a.GetScript(), BigDouble.One);
            });
            Assert.DoesNotThrow(() =>
            {
                engine.EvaluateExpression("method()", "global");
            });
        }

        [Test]
        public void FunctionInvocationCallsMethod()
        {
            int calls = 0;
            engine.RegisterMethod("method", (a, c) =>
            {
                calls++;
                return null;
            });
            engine.EvaluateExpression("method()", "global");
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void FunctionInvocationPassesArguments()
        {
            int calls = 0;
            engine.RegisterMethod("method", (a, c) =>
            {
                calls++;
                Assert.AreEqual(new BigDouble(1), c[0].ToObject<BigDouble>());
                Assert.AreEqual(new BigDouble(2), c[1].ToObject<BigDouble>());
                Assert.AreEqual(new BigDouble(3), c[2].ToObject<BigDouble>());
                return null;
            });
            engine.EvaluateExpression("method(1, 2, 3)", "global");
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void CanUseCustomContext()
        {
            engine.CreateProperty("foo.bar", 5);
            var result = engine.EvaluateExpression("return bar", new Dictionary<string, object>()
            {
                { "bar", engine.GetProperty("foo.bar") }
            });
            Assert.AreEqual(engine.GetProperty("foo.bar"), result);
        }
    }
}