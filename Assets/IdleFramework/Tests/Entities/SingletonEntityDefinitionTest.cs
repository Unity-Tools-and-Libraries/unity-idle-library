using IdleFramework;
using NUnit.Framework;

public class SingletonEntityDefinitionTest
{
    [Test]
    public void SingletonEntityHasAKey()
    {
        var singleton = new SingletonEntityDefinitionBuilder("test").Build();
        Assert.AreEqual("test", singleton.SingletonTypeKey);
    }

    [Test]
    public void SingletonEntityCanHaveMultipleInstances()
    {
        var singletonDef = new SingletonEntityDefinitionBuilder("test").WithInstance()
            .Build();
        Assert.AreEqual(1, singletonDef.Instances.Count);
        Assert.AreEqual("1", singletonDef.GetInstanceByKey("1"));
    }
}
