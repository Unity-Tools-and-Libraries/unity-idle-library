using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using NUnit.Framework;
using UnityEngine;

public class MafTests
{
    [Test]
    public void CalculateSumOfSeries()
    {
        Assert.AreEqual(new BigDouble(3), Maf.SumOfFirstNIntegersInSeries(2));
    }
}
