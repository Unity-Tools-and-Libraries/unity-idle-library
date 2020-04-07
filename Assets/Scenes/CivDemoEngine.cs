using IdleFramework;
using UnityEngine;
using System.Collections.Generic;
using BreakInfinity;
using System.Collections;
using System;

public class Engine : MonoBehaviour
{
    public IdleFramework.IdleEngine framework;
    private bool paused = false;
    // Start is called before the first frame update
    void Awake()
    {
        var configurationBuilder = new GameConfigurationBuilder();

        configurationBuilder.WithCustomGlobalProperty("government", "government", "tribe");

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("population")
            .WithStartingQuantity(2)
            .WithType("resource")
            .WithUpkeepRequirement("food", 1)
            .WithProduction("culture", .1)
            .WithProduction("population", .05));

        Func<string, Func<IdleEngine, BigDouble>> jobQuantityCalculation = jobName => engine =>
        {
            BigDouble population = engine.AllEntities["population"].Quantity;
            SingletonEntityInstance government = engine.GetGlobalSingleton("government");
            var jobWeights = government.GetProperty("jobs").GetAsContainer(engine).Properties;
            BigDouble singleJobWeight = 0;
            BigDouble totalJobWeights = 0;
            foreach (var job in jobWeights)
            {
                if (job.Key == jobName)
                {
                    singleJobWeight = job.Value.GetAsNumber(engine);
                }
                totalJobWeights += job.Value.GetAsNumber(engine);
            }
            return BigDouble.Floor(population * singleJobWeight / totalJobWeights);
        };

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("hunter")
            .WithCalculatedQuantity(new CalculatedNumber(jobQuantityCalculation("hunter")))
            .WithType("job")
            .WithProduction("food", 1));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("food")
            .WithType("resource"));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("culture")
            .WithType("resource"));

        configurationBuilder.WithSingletonEntity(new SingletonEntityDefinitionBuilder("government")
            .CanHaveProperty("jobs")
            .WithInstance(
            new SingletonEntityInstanceBuilder("tribe").WithProperties(new Dictionary<string, ValueContainer>() {
                { "jobs", Literal.Of(new Dictionary<string, ValueContainer>(){
                    { "hunter", Literal.Of(1) }
                }) }
            }))
            );

        framework = new IdleEngine(configurationBuilder.Build());
        Logger.globalLevel = Logger.Level.DEBUG;
        InvokeRepeating("tick", 0f, 1f);
    }

    void tick()
    {
        if (!paused)
        {
            framework.Update(1f);
        }
    }

    void openModal(Modal.ModalValues newModalSettings)
    {
        paused = true;
        this.BroadcastMessage("UpdateModal",
            newModalSettings);
    }

    void modalClosed()
    {
        paused = false;
    }

}
