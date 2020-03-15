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
                new MaxOf(
                    new MinOf(
                        new RatioOf(new EntityPropertyReference("primitive-tools", "quantity"),
                            new EntityPropertyReference("population", "quantity")),
                        new LiteralReference(1)),
                    new LiteralReference(0)))
            .Disabled()
                .When(new EntityPropertyMatcher("population", "quantity", Comparison.LESS_THAN, 10)).Done());

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("culture").WithType("resource")
            .Unbuyable()
            .WithName("Culture")
            .WithRequirement("population", 100)
            .Disabled()
                .When(new EntityPropertyMatcher("population", "quantity", Comparison.LESS_THAN, 100)).Done());

        configurationBuilder.WithModifier(new ModifierDefinitionBuilder("extra-food")
            .Active().When(new EntityPropertyMatcher("food", "quantity", Comparison.GREATER_THAN, new EntityPropertyReference("population", "quantity").Times(new LiteralReference(3)))).And()
            .HasEntityEffect(new EntityPropertyModifierEffect("population", "upkeep", "food", 2, EffectType.MULTIPLY))
            .HasEntityEffect(new EntityPropertyModifierEffect("population", "outputs", "population", 2, EffectType.MULTIPLY))
            .Build());

        configurationBuilder.WithModifier(new ModifierDefinitionBuilder("extraer-food")
            .Active().When(new EntityPropertyMatcher("food", "quantity", Comparison.GREATER_THAN, new EntityPropertyReference("population", "quantity").Times(new LiteralReference(6)))).And()
            .HasEntityEffect(new EntityPropertyModifierEffect("population", "upkeep", "food", 2 , EffectType.MULTIPLY))
            .HasEntityEffect(new EntityPropertyModifierEffect("population", "outputs", "population", 2, EffectType.MULTIPLY))
            .Build());

        configurationBuilder.WithModifier(new ModifierDefinitionBuilder("extraest-food")
            .Active().When(new EntityPropertyMatcher("food", "quantity", Comparison.GREATER_THAN, new EntityPropertyReference("population", "quantity").Times(new LiteralReference(9)))).And()
            .HasEntityEffect(new EntityPropertyModifierEffect("population", "upkeep", "food", 2, EffectType.MULTIPLY))
            .HasEntityEffect(new EntityPropertyModifierEffect("population", "outputs", "population", 2, EffectType.MULTIPLY))
            .Build());

        configurationBuilder.WithModifier(new ModifierDefinitionBuilder("improved-tools")
            .Active().When(new EntityPropertyMatcher("population", "quantity", Comparison.GREATER_THAN, new LiteralReference(100))).And()
            .HasEntityEffect(new EntityPropertyModifierEffect("primitive-tools", "outputs", "food", 2, EffectType.MULTIPLY))
            .HasEntityEffect(new EntityPropertyModifierEffect("primitive-tools", "outputs", "primitive-tools", .1, EffectType.ADD))
            .Build());

        configurationBuilder.WithHook(EngineHookConfigurationBuilder.When().AnyEntity().ProducesAnyEntity().ThenExecute((object hookArgument) =>
        {
            Debug.Log("Hook triggered");
            return hookArgument;
        }));

        framework = new IdleEngine(configurationBuilder.Build());
        InvokeRepeating("tick", 2f, 2f);
    }

    void tick()
    {
        framework.Update();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
