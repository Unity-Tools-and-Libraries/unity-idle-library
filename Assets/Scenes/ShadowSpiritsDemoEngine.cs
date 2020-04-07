using IdleFramework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Weighted_Randomizer;

public class ShadowSpiritsDemoEngine : MonoBehaviour
{
    public IdleFramework.IdleEngine framework;
    private bool paused = false;
    private float tickRate = 1f;
    // Start is called before the first frame update
    void Start()
    {
        GameConfigurationBuilder configurationBuilder = new GameConfigurationBuilder();

        configurationBuilder.WithCustomGlobalProperty("player", "character", "player");
        configurationBuilder.WithCustomGlobalProperty("playerMode", Literal.Of("exploration"));
        configurationBuilder.WithCustomGlobalProperty("activeEncounter");

        configurationBuilder.WithSingletonEntity(new SingletonEntityDefinitionBuilder("character")
            .CanHaveProperty("attributes")
            .WithInstance(new SingletonEntityInstanceBuilder("player")
                .WithProperties(new Dictionary<string, ValueContainer>() {
                    { "attributes", Literal.Of(new Dictionary<string, ValueContainer>(){
                        { "strength",       Literal.Of(10) },
                        { "constitution",   Literal.Of(10) },
                        { "dexterity",      Literal.Of(10) },
                        { "intelligence",   Literal.Of(10) },
                        { "wisdom",         Literal.Of(10) },
                        { "charisma",       Literal.Of(10) }
                        })
                    }
                }))
            );
         
        configurationBuilder.WithSingletonEntity(new SingletonEntityDefinitionBuilder("encounter")
            .CanHaveProperty("type")
            .CanHaveProperty("weight")
            .WithInstance(new SingletonEntityInstanceBuilder("fight")
                .WithProperties(new Dictionary<string, ValueContainer>() {
                    { "type", Literal.Of("combat") },
                    { "weight", Literal.Of(1) }
                }))
            );

        IWeightedRandomizer<string> encounterTable = new DynamicWeightedRandomizer<string>();

        configurationBuilder.WithStartupHook(engine =>
        {
            foreach(var encounter in engine.AllSingletons["encounter"].Instances.Values)
            {
                encounterTable.Add(encounter.InstanceKey, Convert.ToInt32(encounter.GetProperty("weight").GetAsNumber(engine).ToDouble()));
            }
        });

        float randomEncounterChance = .1f;

        configurationBuilder.WithEventHook("encounterStart", (engine, arg) => {
        
        });

        configurationBuilder.WithUpdateHook((IdleEngine engine, float deltaTime) => {
            var mode = engine.GetGlobalStringProperty("playerMode");
            switch(mode)
            {
                case "exploration":
                    float randomEncounterOccuranceCheck = UnityEngine.Random.value;
                    bool randomEncounterOccured = randomEncounterChance.CompareTo(randomEncounterOccuranceCheck) != 1;
                    if(randomEncounterOccured)
                    {
                        engine.DispatchEvent("encounterStart");
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        });

        framework = new IdleEngine(configurationBuilder.Build());
        Logger.globalLevel = Logger.Level.DEBUG;
        InvokeRepeating("tick", 0f, tickRate);
    }

    void tick()
    {
        framework.Update(tickRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
