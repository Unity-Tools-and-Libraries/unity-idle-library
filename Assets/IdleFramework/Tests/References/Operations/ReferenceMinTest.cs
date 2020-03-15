using NUnit.Framework;
using IdleFramework;
using BreakInfinity;

public class ReferenceMinTest
{
    public void MinReturnsSmallestOfValues()
    {
        Assert.IsTrue(BigDouble.Equals(1, new MinOf(new LiteralReference(1), new LiteralReference(2), new LiteralReference(3))));
    }
}
