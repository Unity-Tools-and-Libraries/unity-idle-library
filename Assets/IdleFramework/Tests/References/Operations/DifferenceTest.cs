using BreakInfinity;
using NUnit.Framework;
using IdleFramework;
public class DifferenceTest 
{
    [Test]
    public void returnsDifferenceBetweenTwoValues() {
        Assert.AreEqual(BigDouble.Floor(1), Difference.Between(Literal.Of(2), Literal.Of(1)).GetAsNumber(null));
    }
}
