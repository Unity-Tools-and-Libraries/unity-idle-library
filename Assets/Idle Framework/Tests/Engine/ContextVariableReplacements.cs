using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ContextVariableReplacements
    {
        [Test]
        public void ReplacesVariablesInString()
        {
            Assert.AreEqual("1.2", IdleEngine.ReplacePlaceholders("${foo}.${bar}", new Dictionary<string, string>()
            {
                {"foo", "1" },
                {"bar", "2" }
            }));
        }
    }
}