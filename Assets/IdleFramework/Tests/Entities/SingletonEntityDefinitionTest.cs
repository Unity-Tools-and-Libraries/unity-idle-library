using BreakInfinity;
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
    public void SingletonEntityCanDefineProperties()
    {
        var singleton = new SingletonEntityDefinitionBuilder("test").CanHaveProperty("foo")
            .Build();
        Assert.IsTrue(singleton.DefinesProperty("foo"));
        Assert.IsFalse(singleton.DefinesProperty("bar"));
    }

    [Test]
    public void SingletonEntityInstancesCanHaveDifferentPropertyValues()
    {
        var singleton = new SingletonEntityDefinitionBuilder("test").CanHaveProperty("foo")
            .WithInstance("1").WithProperty("foo", 1).And()
            .WithInstance("2").WithProperty("foo", 2)
            .Build();
        Assert.AreEqual(BigDouble.Floor(1), singleton.Instances["1"].GetPropertyValue("foo"));
        Assert.AreEqual(BigDouble.Floor(2), singleton.Instances["2"].GetPropertyValue("foo"));
    }

    [Test]
    public void SingletonEntityCanHaveMultipleInstances()
    {
        var singletonDef = new SingletonEntityDefinitionBuilder("test").WithInstance("1")
            .Build();
        Assert.AreEqual(1, singletonDef.Instances.Count);
        Assert.IsNotNull(singletonDef.GetInstanceByKey("1"));
    }
}
