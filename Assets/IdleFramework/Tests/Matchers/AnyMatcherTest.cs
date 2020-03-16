using UnityEngine;
using IdleFramework;
using NUnit.Framework;

public class AnyMatcherTest
{
    [Test]
   public void ReturnsTrueIfAllMatchersAreTrue()
    {
        Assert.IsTrue(All.Of(Always.Instance, Always.Instance).Matches(null));
    }

    [Test]
    public void ReturnsFalseIfAnyMatchersAreFalse()
    {
        Assert.IsFalse(All.Of(Always.Instance, Never.Instance).Matches(null));
    }
}
