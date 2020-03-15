using NUnit.Framework;
using IdleFramework;
using BreakInfinity;

public class ReferenceProductTest
{
    [Test]
    public void ProductReturnsLeftTimesRight()
    {
        Assert.AreEqual(BigDouble.Floor(4), new LiteralReference(2).Times(new LiteralReference(2)).Get(null));
    }
}
