using IdleFramework;
using IdleFramework.Configuration;
using IdleFramework.Exceptions;
using NUnit.Framework;

public class EntityPropertyReferenceTest
{
    IdleEngine engine;
    [SetUp]
    public void setup()
    {
        GameConfiguration config = new GameConfigurationBuilder().Build();
        engine = new IdleEngine(config, null);
    }

    [Test]
    public void TestEntityStringPropertyReferenceNoEntity()
    {
        Assert.Throws(typeof(MissingEntityException), () =>
        {
            new EntityStringPropertyReference("foo", "name").Get(engine);
        });
    }

    [Test]
    public void TestEntityNumberPropertyReferenceNoEntity()
    {
        Assert.Throws(typeof(MissingEntityException), () =>
        {
            new EntityNumberPropertyReference("foo", "name").Get(engine);
        });
    }

    [Test]
    public void TestEntityBooleanPropertyReferenceNoEntity()
    {
        Assert.Throws(typeof(MissingEntityException), () =>
        {
            new EntityBooleanPropertyReference("foo", "name").Get(engine);
        });
    }
}
