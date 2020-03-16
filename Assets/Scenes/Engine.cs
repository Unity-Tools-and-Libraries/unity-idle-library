using BreakInfinity;
using IdleFramework;
using System.Collections;
using System.Collections.Generic;
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
            .WithProduction("food", 1.05)
            .WithProduction("culture", .01)
            .WithProduction("primitive-tools", .1)
            .WithProduction("population", .01));

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
                .When(new EntityPropertyMatcher("population", "quantity", Comparison.LESS_THAN, 10)).Done());

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("culture").WithType("resource")
            .Unbuyable()
            .WithName("Culture")
            .WithRequirement("population", 100)
            .HiddenAndDisabled()
                .When(new EntityPropertyMatcher("population", "quantity", Comparison.LESS_THAN, 100)).Done());

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
