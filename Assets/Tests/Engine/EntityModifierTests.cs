using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;

public class EntityModifierTests : TestsRequiringEngine
{
    [SetUp]
    public void Setup()
    {
        base.InitializeEngine();
        UserData.RegisterType<TestEntity>();
        UserData.RegisterType<TestModifier>();
    }

    [Test]
    public void EntityModifierCanSetAFlagOnAnEntity()
    {
        var entity = new TestEntity(engine, 1);
        var modifier = new TestModifier(engine, 2, new Dictionary<string, Tuple<string, string>>()
        {
            { "setFlag", Tuple.Create<string, string>("foo", null) }
        });
        entity.AddModifier(modifier);
        Assert.True(entity.GetFlag("foo"));
    }

    [Test]
    public void EntityModifierWithoutUnapplyScriptDoesntUnapply()
    {
        var entity = new TestEntity(engine, 1);
        var modifier = new TestModifier(engine, 2, new Dictionary<string, Tuple<string, string>>()
        {
            { "setFlag", Tuple.Create<string, string>("foo", null) }
        });
        entity.AddModifier(modifier);
        Assert.True(entity.GetFlag("foo"));
        entity.RemoveModifier(modifier);
        Assert.True(entity.GetFlag("foo"));
    }

    [Test]
    public void UnapplyModifierThatSetsFlagClearsFlag()
    {
        var entity = new TestEntity(engine, 1);
        var modifier = new TestModifier(engine, 2, new Dictionary<string, Tuple<string, string>>()
        {
            { "setFlag", Tuple.Create<string, string>("foo", "foo") }
        });
        entity.AddModifier(modifier);
        entity.RemoveModifier(modifier);
        Assert.False(entity.GetFlag("foo"));
    }

    [Test]
    public void EnityModifierCanClearAFlagOnAnEntity()
    {
        var entity = new TestEntity(engine, 1);
        var modifier = new TestModifier(engine, 2, new Dictionary<string, Tuple<string, string>>()
        {
            { "clearFlag", Tuple.Create<string, string>("foo", null) }
        });
        entity.AddModifier(modifier);
        Assert.False(entity.GetFlag("foo"));
    }

    [Test]
    public void EntityModifierCanModifyAnEntityField()
    {
        var entity = new TestEntity(engine, 1);
        var modifier = new TestModifier(engine, 2, new Dictionary<string, Tuple<string, string>>()
        {
            { "foo", Tuple.Create<string, string>("value + 1", "value -1") }
        });
        entity.AddModifier(modifier);
        Assert.AreEqual(new BigDouble(1), entity.foo);
    }

    [Test]
    public void EntityModifierCanModifyAnEntityProperty()
    {
        var entity = new TestEntity(engine, 1);
        var modifier = new TestModifier(engine, 2, new Dictionary<string, Tuple<string, string>>()
        {
            { "bar", Tuple.Create<string, string>("value + 1", "value -1") }
        });
        entity.AddModifier(modifier);
        Assert.AreEqual(new BigDouble(2), entity.Bar);
    }
}
