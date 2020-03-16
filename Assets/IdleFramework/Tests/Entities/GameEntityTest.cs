using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class GameEntityTest 
{
    [Test]
    public void GameEntityQuantityIsLimitedByCap()
    {
        var entity = new GameEntity(new EntityDefinitionBuilder(null).QuantityCappedBy(Literal.Of(1)).Build(), null);
        entity.SetQuantity(100);
        Assert.AreEqual(BigDouble.Floor(1), entity.Quantity);
    }
}
