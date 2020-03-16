using NUnit.Framework;
using IdleFramework;
using BreakInfinity;

public class ReferenceMinTest
{
    public void MinReturnsSmallestOfValues()
    {
        Assert.IsTrue(BigDouble.Equals(1, Min.Of(Literal.Of(1), Literal.Of(2), Literal.Of(3))));
    }
}
