using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Paths
{
    public class PathValidationTests
    {
        [Test]
        public void EmptyPathFailsValidation()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IdleEngine.ValidatePath("");
            });
        }
    }
}