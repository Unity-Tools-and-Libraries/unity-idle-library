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
            engine.RegisterMethod("method", (ie, vc, ev) => throw new System.Exception());
            Assert.Throws(typeof(Exception), () =>
            {
                engine.InvokeMethod("method", null, null);
            });
        }

        [Test]
        public void CallingUnknownMethodThrowsException()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                engine.InvokeMethod("method", null, null);
            });
        }
    }
}