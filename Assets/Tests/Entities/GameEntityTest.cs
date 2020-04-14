using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class GameEntityTest 
{
    IdleEngine engine;
    [SetUp]
    public void setup()
    {
        var configuration = new GameConfigurationBuilder().WithEntity(new EntityDefinitionBuilder("").QuantityCappedBy(Literal.Of(1)));
        engine = new IdleEngine(configuration.Build(), null);
    }
    [Test]
    public void GameEntityQuantityIsLimitedByCap()
    {
        var entity = engine.GetEntity("");
        entity.SetQuantity(100);
        Assert.AreEqual(BigDouble.Floor(1), entity.Quantity);
    }

    [Test]
    public void GameEntityPredefinedPropertiesCanBeRetreivedByName()
    {
        var entity = engine.GetEntity("");
        entity.GetStringProperty("Name");
        entity.GetNumberProperty("Quantity");
        entity.GetNumberProperty("QuantityCap");
        entity.GetNumberProperty("QuantityChangePerSecond");
        entity.GetMapProperty("FixedInputs");
        entity.GetMapProperty("FixedOutputs");
        entity.GetMapProperty("ScaledInputs");
        entity.GetMapProperty("ScaledOutputs");
    }
}
