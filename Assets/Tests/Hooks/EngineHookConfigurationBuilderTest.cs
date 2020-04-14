using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

public class EngineHookConfigurationBuilderTest {
    [Test]
    public void HookConfigurationCanSpecifyAnEntityForTrigger()
    {
        EngineHookDefinitionProperties<Entity, BigDouble> config = new EntityProductionHook.Builder().WhenEntity("foo").ProducesAnyEntity().ThenExecute(null).Build();
        Assert.AreEqual("foo", config.Actor);
        Assert.AreEqual("*", config.Subject);
        Assert.AreEqual(EngineHookEvent.WILL_PRODUCE, config.Action);
    }

    [Test]
    public void HookConfigurationCanSpecifyProductionOfAnyEntityForTrigger()
    {
        EngineHookDefinitionProperties<Entity, BigDouble> config = new EntityProductionHook.Builder().WhenAnyEntity().ProducesAnyEntity().ThenExecute(null).Build();
        Assert.AreEqual(EngineHookEvent.WILL_PRODUCE, config.Action);
        Assert.AreEqual("*", config.Subject);
        Assert.AreEqual("*", config.Actor);
    }
}
