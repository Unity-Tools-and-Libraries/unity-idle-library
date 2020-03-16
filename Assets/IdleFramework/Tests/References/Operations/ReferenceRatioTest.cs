using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class ReferenceRatioTest 
{
    [Test]
    public void RatioReturnsLeftDividedByRight()
    {
        Assert.AreEqual(BigDouble.Floor(1), new RatioOf(Literal.Of(2), Literal.Of(2)).Get(null));
    }

    [Test]
    public void RatioReturnsNaNIfRightIs0()
    {
        Assert.IsTrue(BigDouble.IsNaN(new RatioOf(Literal.Of(2), Literal.Of(0)).Get(null)));
    }
}
