using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitModifiers : RequiresEngineTests
{
    [Test]
    public void MaximumModifierReturnsValueIfInputIsHigher()
    {
        var modifier = new ValueModifier("", "", "return math.min(5, value)", engine);
        var output = modifier.Apply(engine, null, new BigDouble(10));
        Assert.AreEqual(new BigDouble(5), output);
    }

    [Test]
    public void MaximumModifierReturnsEvaluatedValueIfInputIsHigher()
    {
        var modifier = new ValueModifier("", "", "return math.min(5, value)", engine);
        var output = modifier.Apply(engine, null, new BigDouble(10));
        Assert.AreEqual(new BigDouble(5), output);
    }

    [Test]
    public void MinimumValueModifierReturnsFloorIfInputIsLess()
    {
        var modifier = new ValueModifier("", "", "return math.max(5, value)", engine);
        var output = modifier.Apply(engine, null, BigDouble.Zero);
        Assert.AreEqual(new BigDouble(5), output);
    }

    [Test]
    public void MinimumValueModifierReturnsEvaluatedValueIfInputIsLess()
    {
        var modifier = new ValueModifier("", "", "return math.max(5, value)", engine);
        var output = modifier.Apply(engine, null, BigDouble.Zero);
        Assert.AreEqual(new BigDouble(5), output);
    }

    [Test]
    public void ClampModifierReturnsFloorIfInputIsLess()
    {
        var modifier = new ValueModifier("", "", "return math.clamp(value, 5, 10)", engine);
        var output = modifier.Apply(engine, null, BigDouble.Zero);
        Assert.AreEqual(new BigDouble(5), output);
    }

    [Test]
    public void ClampModifierReturnsCeilingIfInputIsMore()
    {
        var modifier = new ValueModifier("", "", "return math.clamp(value, 5, 10)", engine);
        var output = modifier.Apply(engine, null, new BigDouble(15));
        Assert.AreEqual(new BigDouble(10), output);
    }
}
