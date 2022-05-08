using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class ValueContainerConversionTests : RequiresEngineTests
    {
        [Test]
        public void StringToValueContainer()
        {
            ValueContainer container = engine.CreateValueContainer("string");
            Assert.AreEqual("string", container.ValueAsString());
        }

        [Test]
        public void NumberToValueContainer()
        {
            ValueContainer container = engine.CreateValueContainer(BigDouble.One);
            Assert.AreEqual(BigDouble.One, container.ValueAsNumber());
        }

        [Test]
        public void BoolToValueContainer()
        {
            ValueContainer container = engine.CreateValueContainer(true);
            Assert.AreEqual(true, container.ValueAsBool());
        }
    }
}