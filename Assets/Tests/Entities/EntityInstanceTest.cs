
using NUnit.Framework;
using IdleFramework;
public class EntityInstanceTest
{
    Entity entity;
    Entity variant;

    [SetUp]
    public void setup()
    {
        var configuration = new GameConfigurationBuilder().WithEntity(new EntityDefinitionBuilder("")
            .WithFixedInput("", 1)
            .WithVariant(new EntityDefinitionBuilder("foo"))).Build();
        var engine = new IdleEngine(configuration, null);
        engine.Update(1f);
        entity = engine.GetEntity("");
        variant = entity.Variants["foo"];
    }

    [Test]
    public void InstanceHasSameFixedOutputByDefault()
    {
        Assert.AreEqual(entity.FixedOutputs.Count, variant.FixedOutputs.Count);
        foreach(var fixedOutput in entity.FixedOutputs)
        {
            Assert.AreEqual(fixedOutput.Value, variant.FixedOutputs[fixedOutput.Key]);
        }
        Assert.AreEqual(entity.FixedInputs.Count, variant.FixedInputs.Count);
        foreach (var fixedInput in entity.FixedInputs)
        {
            Assert.AreEqual(fixedInput.Value, variant.FixedInputs[fixedInput.Key]);
        }
        Assert.AreEqual(entity.ScaledOutputs.Count, variant.ScaledOutputs.Count);
        foreach (var scaledOutput in entity.ScaledOutputs)
        {
            Assert.AreEqual(scaledOutput.Value, variant.ScaledOutputs[scaledOutput.Key]);
        }
        Assert.AreEqual(entity.ScaledInputs.Count, variant.ScaledInputs.Count);
        foreach (var scaledInput in entity.ScaledInputs)
        {
            Assert.AreEqual(scaledInput.Value, variant.ScaledInputs[scaledInput.Key]);
        }
    }
}
