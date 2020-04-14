using IdleFramework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyMatcherTest
{
    [Test]
    public void AnyMatcherReturnsTrueIfAnyMatcherIsTrue()
    {
        Assert.IsTrue(Any.Of(Always.Instance, Never.Instance).Matches(null));
    }
    [Test]
    public void AnyMatcherReturnsFalseIfAllMatchersAreFalse()
    {
        Assert.IsFalse(Any.Of(Never.Instance, Never.Instance).Matches(null));
    }
}
