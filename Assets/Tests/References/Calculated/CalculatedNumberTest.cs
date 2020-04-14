using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class CalculatedNumberTest
{
    private CalculatedNumber calculated;
    [SetUp]
    public void setup()
    {
        calculated = new CalculatedNumber(engine =>
        {
            return 1;
        });
    }

    [Test]
    public void getNumberReturnsCalculatedNumber()
    {
        Assert.AreEqual(BigDouble.Floor(1), calculated.Get(null));
    }
}
