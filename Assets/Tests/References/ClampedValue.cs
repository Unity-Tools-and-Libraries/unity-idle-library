using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class ClampedValueTest
{
    private Clamped tooLow;
    private Clamped tooHigh;
    private Clamped baseValue;
    [SetUp]
    public void setup()
    {
        tooLow = Clamped.Of(Literal.Of(-1), Literal.Of(0), Literal.Of(1));
        tooHigh = Clamped.Of(Literal.Of(2), Literal.Of(0), Literal.Of(1));
        baseValue = Clamped.Of(Literal.Of(1), Literal.Of(0), Literal.Of(2));
    }
    
    [Test]
    public void returnMinimumWhenValueIsLessThan()
    {
        Assert.AreEqual(BigDouble.Floor(0), tooLow.Get(null));
    }

    [Test]
    public void returnMaximumWhenValueIsLessThan()
    {
        Assert.AreEqual(BigDouble.Floor(1), tooHigh.Get(null));
    }

    [Test]
    public void returnActualValueWhenInBetween()
    {
        Assert.AreEqual(BigDouble.Floor(1), baseValue.Get(null));
    }
}
