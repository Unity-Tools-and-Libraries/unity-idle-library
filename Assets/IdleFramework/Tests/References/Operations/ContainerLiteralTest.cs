using BreakInfinity;
using IdleFramework;
using NUnit.Framework;
using System.Collections.Generic;

public class ContainerLiteralTest
{
    private ContainerLiteral container;
    [SetUp]
    public void setup()
    {
        container = new ContainerLiteral(new Dictionary<string, ValueContainer>() {
            { "foo", Literal.Of(0) },
            { "bar", Literal.Of(true) },
            { "baz", Literal.Of("string") }
        });
    }
    [Test]
    public void containerHasDefinedProperties()
    {
        Assert.AreEqual(BigDouble.Floor(0), container.GetAsContainer(null).Get("foo").GetAsNumber(null));
        Assert.IsTrue(container.GetAsContainer(null).Get("bar").GetAsBoolean(null));
        Assert.AreEqual("string", container.GetAsContainer(null).Get("baz").GetAsString(null));
    }
}
