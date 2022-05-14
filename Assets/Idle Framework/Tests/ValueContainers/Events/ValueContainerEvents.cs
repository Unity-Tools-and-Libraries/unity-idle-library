using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueContainerEvents : RequiresEngineTests
{
    [Test]
    public void WatchingForChangesReceivesCurrentValueImmediately()
    {
        var valueReference = engine.CreateValueContainer();
        bool called = false;
        valueReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newValue => called = true);
        Assert.IsTrue(called);
    }

    [Test]
    public void ChangingValueOfWatchedValueNotifiesListeners()
    {
        var valueReference = engine.CreateValueContainer();
        int calls = 0;
        valueReference.Subscribe(ValueContainer.Events.VALUE_CHANGED, newValue =>
        {
            calls++;
            if (calls == 1)
            {
                Assert.AreEqual(BigDouble.Zero, newValue);
            }
            else
            {
                Assert.AreEqual(BigDouble.One, newValue);
            }
        });
        valueReference.Set(BigDouble.One);
        Assert.AreEqual(2, calls);
    }
}
