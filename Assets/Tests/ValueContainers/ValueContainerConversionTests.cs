using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueContainerConversionTests
    {
        [Test]
        public void StringToValueContainer()
        {
            ValueContainer container = "string";
            Assert.AreEqual("string", container.ValueAsString());
        }

        [Test]
        public void NumberToValueContainer()
        {
            ValueContainer container = BigDouble.One;
            Assert.AreEqual(BigDouble.One, container.ValueAsNumber());
        }

        [Test]
        public void BoolToValueContainer()
        {
            ValueContainer container = true;
            Assert.AreEqual(true, container.ValueAsBool());
        }
    }
}