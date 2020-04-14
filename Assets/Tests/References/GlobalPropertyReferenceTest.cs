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
            .WithCustomGlobalProperty("string", Literal.Of("astring"))
            .Build();
        engine = new IdleEngine(config, null);
    }

    [Test]
    public void ThrowExceptionForMissingNumberGlobalProperty()
    {
        Assert.Throws(typeof(MissingGlobalPropertyException), () =>
        {
            new GlobalNumberPropertyReference("foo").Get(engine);
        });
    }
    [Test]
    public void ThrowExceptionForMissingStringGlobalProperty()
    {
        Assert.Throws(typeof(MissingGlobalPropertyException), () =>
        {
            new GlobalStringPropertyReference("foo").Get(engine);
        });
    }
    [Test]
    public void ThrowExceptionForMissingBooleanGlobalProperty()
    {
        Assert.Throws(typeof(MissingGlobalPropertyException), () =>
        {
            new GlobalBooleanPropertyReference("foo").Get(engine);
        });
    }

    [Test]
    public void ReturnsTheValueOfAGlobalStringProperty()
    {
        Assert.AreEqual("astring", new GlobalStringPropertyReference("string").Get(engine));
    }
}
