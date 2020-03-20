using BreakInfinity;
using IdleFramework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceMaxTest
{
    [Test]
    public void MaxReturnsLargestValue()
    {
        Assert.AreEqual(BigDouble.Floor(3), Max.Of(Literal.Of(1), Literal.Of(2), Literal.Of(3)).GetAsNumber(null));
    }
}
