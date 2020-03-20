using NUnit.Framework;
using IdleFramework;
using BreakInfinity;

public class ReferenceMinTest
{
    [Test]
    public void MinReturnsSmallestOfValues()
    {
        Assert.AreEqual(BigDouble.Floor(1), Min.Of(Literal.Of(1), Literal.Of(2), Literal.Of(3)).GetAsNumber(null));
    }
}
