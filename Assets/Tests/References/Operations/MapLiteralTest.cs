using BreakInfinity;
using IdleFramework;
using NUnit.Framework;
using System.Collections.Generic;

public class MapLiteralTest
{
    private MapLiteral container;
    [SetUp]
    public void setup()
    {
        container = new MapLiteral(new Dictionary<string, ValueContainer>() {
            { "foo", Literal.Of(0) },
            { "bar", Literal.Of(true)},
            { "baz", Literal.Of("string") }
        });
    }
    [Test]
    public void containerHasDefinedProperties()
    {
        Assert.AreEqual(BigDouble.Floor(0), container.Get(null)["foo"].AsNumber().Get(null));
        Assert.IsTrue(container.Get(null)["bar"].AsBoolean().Get(null));
        Assert.AreEqual("string", container.Get(null)["baz"].AsString().Get(null));
    }
}
