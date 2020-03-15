using BreakInfinity;
using IdleFramework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityTest 
{
    [Test]
    public void GameEntityQuantityIsLimitedByCap()
    {
        var entity = new GameEntity(new EntityDefinitionBuilder(null).QuantityCappedBy(new LiteralReference(1)).Build(), null);
        entity.SetQuantity(100);
        Assert.AreEqual(BigDouble.Floor(1), entity.Quantity);
    }
}
