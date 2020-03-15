using IdleFramework;
using NUnit.Framework;

public class EngineHookConfigurationBuilderTest {
    [Test]
    public void HookConfigurationCanSpecifyAnEntityForTrigger()
    {
        EngineHookDefinitionProperties config = EngineHookConfigurationBuilder.When().AnyEntity().WillProduceAnyEntity().ThenExecute(EngineHookConfigurationBuilder.Logs<object>("Message"));
        Assert.AreEqual("*", config.Actor);
    }

    [Test]
    public void HookConfigurationCanSpecifyProductionOfAnyEntityForTrigger()
    {
        EngineHookDefinitionProperties config = EngineHookConfigurationBuilder.When().AnyEntity().WillProduceAnyEntity().ThenExecute(EngineHookConfigurationBuilder.Logs<object>("Message")).Build();
        Assert.AreEqual(EngineHookAction.WILL_PRODUCE, config.Action);
        Assert.AreEqual("*", config.Subject);
    }
}
