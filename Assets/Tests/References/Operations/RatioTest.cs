using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class RatioTest 
{
    [Test]
    public void RatioReturnsLeftDividedByRight()
    {
        Assert.AreEqual(BigDouble.Floor(1), Ratio.Of(Literal.Of(2), Literal.Of(2)).Get(null));
    }

    [Test]
    public void RatioReturnsNaNIfRightIs0()
    {
        Assert.IsTrue(BigDouble.IsNaN(Ratio.Of(Literal.Of(2), Literal.Of(0)).Get(null)));
    }
}
