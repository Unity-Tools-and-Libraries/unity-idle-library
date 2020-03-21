using IdleFramework;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public IdleFramework.IdleEngine framework;
    // Start is called before the first frame update
    void Awake()
    {
        var configurationBuilder = new GameConfigurationBuilder();
        configurationBuilder.WithEntity(new EntityDefinitionBuilder("population").WithType("resource").WithType("producer")
            .WithName("Population")
            .Unbuyable()
            .WithStartingQuantity(5)
            .WithCost("food", 100)
            .WithUpkeepRequirement("food", 1)
            .WithOutput("food", 1.05)
            .WithOutput("culture", .01)
            .WithOutput("primitive-tools", .1)
            .WithOutput("population", .01));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("food").WithType("resource")
            .WithName("Food")
            .WithStartingQuantity(5));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("primitive-tools").WithType("resource")
            .WithName("Primitive Tools")
            .WithConsumption("primitive-tools", .1)
            .QuantityCappedBy(new EntityPropertyReference("population", "quantity"))
            .WithProduction("food",
                Max.Of(
                    Min.Of(
                        new RatioOf(new EntityPropertyReference("primitive-tools", "quantity"),
                            new EntityPropertyReference("population", "quantity")),
                        Literal.Of(1)),
                    Literal.Of(0)))
            .HiddenAndDisabled()
                .When(new EntityNumberPropertyMatcher("population", "quantity", Comparison.LESS_THAN, Literal.Of(20)).Or(
                    new EntityNumberPropertyMatcher("population", "quantity", Comparison.GREATER_THAN, Literal.Of(10)))).Done());

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("culture").WithType("resource")
            .Unbuyable()
            .WithName("Culture")
            .WithRequirement("population", 100)
            .HiddenAndDisabled()
                .When(new EntityNumberPropertyMatcher("population", "quantity", Comparison.LESS_THAN, Literal.Of(100))).Done());

        configurationBuilder.WithAchievement(new AchievementConfigurationBuilder("pop-10").GainedWhen(
            new EntityNumberPropertyMatcher("population", "quantity", Comparison.GREATER_THAN, Literal.Of(10))));

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder().WhenGameStarts().ThenExecute(()=>
        {
            Debug.Log("Game start hook executed.");
        }));

        framework = new IdleEngine(configurationBuilder.Build());
        InvokeRepeating("tick", 0f, .25f);
    }

    void tick()
    {
        framework.Update(.25f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
