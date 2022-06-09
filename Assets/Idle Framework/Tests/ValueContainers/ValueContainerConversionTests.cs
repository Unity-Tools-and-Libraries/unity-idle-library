using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Tests.ValueContainers.Conversions
{
    public class ValueContainerConversionTests : RequiresEngineTests
    {
        [Test]
        public void StringToBool()
        {
            ValueContainer container = engine.CreateProperty("path", "string");
            Assert.IsTrue(container.ValueAsBool());

            container.Set("");
            Assert.IsFalse(container.ValueAsBool());

        }

        //[Test]
        public void NumberToValueContainer()
        {
            ValueContainer container = engine.CreateProperty("path", BigDouble.One);
            Assert.AreEqual(BigDouble.One, container.ValueAsNumber());
        }

        //[Test]
        public void BoolToValueContainer()
        {
            ValueContainer container = engine.CreateProperty("path", true);
            Assert.AreEqual(true, container.ValueAsBool());
        }

        // Boolean
        [Test]
        public void GettingBoolAsBoolReturnsAsIs()
        {
            Assert.IsTrue(engine.CreateProperty("path", true).ValueAsBool());
            Assert.IsFalse(engine.CreateProperty("path", false).ValueAsBool());
        }

        [Test]
        public void GettingNullAsBoolReturnsFalse()
        {
            Assert.IsFalse(engine.CreateProperty("path", (string)null).ValueAsBool());
        }

        [Test]
        public void GettingNumberAsBool()
        {
            Assert.AreEqual(BigDouble.One, engine.CreateProperty("path", true).ValueAsNumber());
            Assert.AreEqual(BigDouble.Zero, engine.CreateProperty("path", false).ValueAsNumber());
        }

        [Test]
        public void GettingMapAsBoolReturnsTrueIfNotNull()
        {
            Assert.IsFalse(engine.CreateProperty("path", null as Dictionary<string, ValueContainer>).ValueAsBool());
            Assert.IsTrue(engine.CreateProperty("path", new Dictionary<string, ValueContainer>()).ValueAsBool());
        }

        [Test]
        public void CanImplicitlyConvertToBool()
        {
            Assert.IsTrue(engine.CreateProperty("path", true));
        }

        // Number
        [Test]
        public void GettingNumberAsNumberReturnsAsIs()
        {
            Assert.AreEqual(new BigDouble(100), engine.CreateProperty("path", new BigDouble(100)).ValueAsNumber());
        }

        [Test]
        public void GettingNumberAsStringReturnsAsIs()
        {
            Assert.AreEqual("100", engine.CreateProperty("path", new BigDouble(100)).ValueAsString());
        }

        [Test]
        public void GettingNumberAsBoolReturnsTrueForNonZero()
        {
            Assert.IsTrue(engine.CreateProperty("path", new BigDouble(1)).ValueAsBool());
            Assert.IsTrue(engine.CreateProperty("path", new BigDouble(-1)).ValueAsBool());
            Assert.IsFalse(engine.CreateProperty("path", BigDouble.Zero).ValueAsBool());
        }

        [Test]
        public void GettingMapAsNumberReturnsZero()
        {
            Assert.AreEqual(BigDouble.Zero,
                engine.CreateProperty("path", new Dictionary<string, ValueContainer>()).ValueAsNumber());
        }

        [Test]
        public void RawValueReturnsActualValue()
        {
            var booleanReference = engine.CreateProperty("1", true);
            var numberReference = engine.CreateProperty("2", BigDouble.One);
            var stringReference = engine.CreateProperty("3", "string");
            var mapReference = engine.CreateProperty("4", new Dictionary<string, ValueContainer>());

            Assert.AreEqual(true, booleanReference.ValueAsRaw());
            Assert.AreEqual(BigDouble.One, numberReference.ValueAsRaw());
            Assert.AreEqual("string", stringReference.ValueAsRaw());
            Assert.AreEqual(new Dictionary<string, ValueContainer>(), mapReference.ValueAsRaw());
        }
    }
}