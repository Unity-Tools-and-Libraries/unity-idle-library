using NUnit.Framework;
using IdleFramework;
using BreakInfinity;

public class ReferenceProductTest
{
    [Test]
    public void ProductReturnsLeftTimesRight()
    {
        Assert.AreEqual(BigDouble.Floor(4), Literal.Of(2).Times(Literal.Of(2)).GetAsNumber(null));
    }
}
