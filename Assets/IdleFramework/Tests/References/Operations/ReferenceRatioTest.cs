using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class ReferenceRatioTest 
{
    [Test]
    public void RatioReturnsLeftDividedByRight()
    {
        Assert.AreEqual(BigDouble.Floor(1), new RatioOf(new LiteralReference(2), new LiteralReference(2)).Get(null));
    }

    [Test]
    public void RatioReturnsNaNIfRightIs0()
    {
        Assert.IsTrue(BigDouble.IsNaN(new RatioOf(new LiteralReference(2), new LiteralReference(0)).Get(null)));
    }
}
