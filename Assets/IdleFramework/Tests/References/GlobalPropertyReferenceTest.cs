using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class GlobalPropertyReferenceTest
{
    private IdleEngine engine;
    [SetUp]
    public void setup()
    {
        var config = new GameConfigurationBuilder()
            .WithCustomGlobalProperty("exists", Literal.Of(true))
            .WithCustomGlobalProperty("foo", Literal.Of("string"))
            .Build();
        engine = new IdleEngine(config);
    }
    [Test]
    public void Returns0ForANonexistantNumberProperty()
    {
        Assert.AreEqual(BigDouble.Zero, new GlobalPropertyReference("foo").GetAsNumber(engine));
    }
    [Test]
    public void ReturnsTheValueOfAGlobalNumberProperty()
    {
        Assert.IsTrue(new GlobalPropertyReference("exists").GetAsBoolean(engine));
    }
    [Test]
    public void ReturnsTheValueOfAGlobalStringProperty()
    {
        Assert.AreEqual("string", new GlobalPropertyReference("foo").GetAsString(engine));
    }
    [Test]
    public void ReturnsTheValueOfTheGlobalStringProperty()
    {
        Assert.AreEqual("string", new GlobalPropertyReference("foo").GetAsString(engine));
    }
}
