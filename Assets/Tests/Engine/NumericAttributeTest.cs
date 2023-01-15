using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.Engine
{
    public class NumericAttributeTest
    {
        [Test]
        public void TotalIsCalculated()
        {
            var attribute = new NumericAttribute(1, 1, 2, 2);
            Assert.AreEqual(new BigDouble(3), attribute.Total);
        }
    }
}