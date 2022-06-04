using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers.Conversions
{
    public class ValueNormalizationTests
    {
        [Test]
        public void FloatNormalizesToBigDouble()
        {
            Assert.AreEqual(BigDouble.One, ValueContainer.NormalizeValue(1f));
        }

        [Test]
        public void IntNormalizesToBigDouble()
        {
            Assert.AreEqual(BigDouble.One, ValueContainer.NormalizeValue(1));
        }

        [Test]
        public void DoubleNormalizesToBigDouble()
        {
            Assert.AreEqual(BigDouble.One, ValueContainer.NormalizeValue(1.0));
        }

        [Test]
        public void LongNormalizesToBigDouble()
        {
            Assert.AreEqual(BigDouble.One, ValueContainer.NormalizeValue(1L));
        }

        [Test]
        public void DecimalNormalizesToBigDouble()
        {
            Assert.AreEqual(BigDouble.One, ValueContainer.NormalizeValue(1m));
        }
    }
}