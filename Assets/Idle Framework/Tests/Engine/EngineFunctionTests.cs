using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class EngineFunctionTests : RequiresEngineTests
    {
        [Test]
        public void ExceptionThrownFromMethodBubblesUp()
        {
            LogAssert.ignoreFailingMessages = true;
            engine.RegisterMethod("method", (ie, ev) => throw new System.Exception());
            Assert.Throws(typeof(Exception), () =>
            {
                engine.EvaluateExpression("method()", "global");
            });
        }

        [Test]
        public void CallingUnknownMethodThrowsException()
        {
            Assert.Throws(typeof(ScriptRuntimeException), () =>
            {
                engine.EvaluateExpression("method()");
            });
        }

        [Test]
        public void EvaluateNullExpressionThrows()
        {
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                engine.EvaluateExpression(null);
            });
        }
    }
}