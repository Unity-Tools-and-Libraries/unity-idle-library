using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PropertiesHolderTests
{
    private PropertiesHolder holder;

    [SetUp]
    public void Setup()
    {
        holder = new PropertiesHolder();
    }

    [Test]
    public void HolderCanReturnKeys()
    {
        holder.Add("foo", "bar");
        Assert.AreEqual(new List<string>() { "foo" }, holder.Keys);
    }

    [Test]
    public void CanAddAKeyValuePair()
    {
        holder.Add(new KeyValuePair<string, object>("foo", "bar"));
        Assert.AreEqual(new List<string>() { "foo" }, holder.Keys);
        Assert.AreEqual(new List<string>() { "bar" }, holder.Values);
        Assert.AreEqual(1, holder.Count);
    }

    [Test]
    public void HolderCanReturnValues()
    {
        holder.Add("foo", "bar");
        Assert.AreEqual(new List<string>() { "bar" }, holder.Values);
    }

    [Test]
    public void HolderCanReturnCount()
    {
        holder.Add("foo", "bar");
        Assert.AreEqual(1, holder.Count);
    }

    [Test]
    public void HolderCanClearItsContents()
    {
        holder.Add("foo", "bar");
        holder.Clear();
        Assert.AreEqual(0, holder.Count);
        Assert.AreEqual(new List<string>(), holder.Keys);
        Assert.AreEqual(new List<object>(), holder.Values);
    }

    [Test]
    public void CanGetEnumerator()
    {
        var enumerator = holder.GetEnumerator();
        Assert.NotNull(enumerator);
    }
}
