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
            .WithInstance(SingletonEntityInstanceBuilder.For("1").WithProperties(new System.Collections.Generic.Dictionary<string, ValueContainer> {
                {"foo", Literal.Of(1) } }))
            .WithInstance(SingletonEntityInstanceBuilder.For("2").WithProperties(new System.Collections.Generic.Dictionary<string, ValueContainer> {
                {"foo", Literal.Of(2) } }))
            .Build();
        Assert.AreEqual(BigDouble.Floor(1), singleton.Instances["1"].GetProperty("foo").GetAsNumber(null));
        Assert.AreEqual(BigDouble.Floor(2), singleton.Instances["2"].GetProperty("foo").GetAsNumber(null));
    }

    [Test]
    public void SingletonEntityCanHaveMultipleInstances()
    {
        var singletonDef = new SingletonEntityDefinitionBuilder("test").WithInstance(SingletonEntityInstanceBuilder.For("1"))
            .Build();
        Assert.AreEqual(1, singletonDef.Instances.Count);
        Assert.IsNotNull(singletonDef.GetInstanceByKey("1"));
    }
}
