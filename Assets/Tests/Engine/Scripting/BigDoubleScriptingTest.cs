using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using NUnit.Framework;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Scripting
{
    public class BigDoubleScriptingTest : TestsRequiringEngine
    {
        [Test]
        public void Concatenation()
        {
            Assert.AreEqual("12", engine.Scripting.EvaluateStringAsScript("return '1' .. x", Tuple.Create<string, object>("x", new BigDouble(2))).String);
        }
    }
}