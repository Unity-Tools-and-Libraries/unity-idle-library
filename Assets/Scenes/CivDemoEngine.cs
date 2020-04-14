using IdleFramework;
using IdleFramework.UI.Events.Payloads;
using UnityEngine;
using System.Collections.Generic;
using BreakInfinity;
using System.Linq;
using IdleFramework.State.Matchers;

public class CivDemoEngine : MonoBehaviour
{
    public IdleFramework.IdleEngine framework;
    private bool paused = false;
    // Start is called before the first frame update
    void Awake()
    {
        var configurationBuilder = new GameConfigurationBuilder();

        configurationBuilder.WithCustomGlobalProperty("government", Literal.Of("citystate"));

        configurationBuilder.WithBeforeUpdateHook((engine, deltaTime) =>
        {
            BigDouble population = engine.GetEntity("population").Quantity;
            string governmentName = engine.GetGlobalStringProperty("government").Get(engine);
            Entity government = engine.GetEntity("government").GetVariant(governmentName);
            var jobWeights = government.GetMapProperty("jobs");
            IList<string> jobsSortedByWeight = new List<string>(
                jobWeights.AsEnumerable()
                    .OrderBy(e => e.Value.AsNumber().Get(engine))
                    .Select(e => e.Key)
                );
            Dictionary<string, BigDouble> assignedPopulation = new Dictionary<string, BigDouble>();
            BigDouble totalWeight = 0;
            foreach(var weight in jobWeights.Values)
            {
                totalWeight += weight.AsNumber().Get(engine);
            }
            BigDouble accumulatedRemainder = 0;
            foreach(var job in jobsSortedByWeight)
            {
                BigDouble calculatedQuantity = population * (jobWeights[job].AsNumber().Get(engine) / totalWeight);
                assignedPopulation[job] = BigDouble.Floor(calculatedQuantity);
                accumulatedRemainder = calculatedQuantity - assignedPopulation[job];
            }
            foreach (var job in jobsSortedByWeight)
            {
                assignedPopulation[job] = assignedPopulation[job] + 1;
                accumulatedRemainder -= 1;
                if(accumulatedRemainder <= 0)
                {
                    break;
                }
            }
            foreach(var job in assignedPopulation)
            {
                engine.GetEntity(job.Key).SetQuantity(job.Value);
            }
        });

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("population")
            .WithStartingQuantity(2)
            .WithType("resource")
            .WithUpkeepRequirement("food", 1)
            .WithScaledOutput("culture", .1)
            .WithScaledOutput("population", .05));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("farmer")
            .WithType("job")
            .WithScaledOutput("food", 2));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("craftsman")
            .WithType("job")
            .WithScaledOutput("tools", 1)
            .WithScaledOutput("weapons", 1));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("food")
            .WithType("resource")
            .DoesNotAccumulate());

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("culture")
            .WithType("resource"));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("government")
            .WithCustomMapProperty("jobs", new Dictionary<string, ValueContainer>()
            {
                { "hunter", Literal.Of(1) }
            })
            .WithVariant(
                new EntityDefinitionBuilder("citystate")
                .WithCustomMapProperty("jobs", new Dictionary<string, ValueContainer>()
                {
                    { "farmer", Literal.Of(3) },
                    { "craftsman", Literal.Of(2) },
                    { "warrior", Literal.Of(1) }
                })
            ));

        configurationBuilder.WithEntityProductionHook();

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder("intro")
            .InitiatesWhen(Always.Instance)
            .ExecutesHookOnInitiation(engine =>
            {
                engine.Events.DispatchEvent(new OpenPopupPayload("Welcome", "You are seeking to lead a civilization to conquer the world... And beyond!"));
            }).CompletesWhen(Always.Instance));

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder("population")
            .InitiatesWhen(new TutorialCompleteMatcher("intro"))
            .ExecutesHookOnCompletion(engine =>
            {
                engine.Events.DispatchEvent(new OpenPopupPayload("Population and Jobs", 
                    "The basis of your civilization is your population. These are the people that do the work of growing the food, constructing the buildings, manufacturing the tools and fighting the battles.\n" +
                    "Jobs are the tasks that your population does. In early history, farmers grow food, craftsmen manufacture tools and weapons and soldiers fight battles."));
            }));

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder("resources")
            .InitiatesWhen(new TutorialCompleteMatcher("population"))
            .ExecutesHookOnCompletion(engine => {
                engine.Events.DispatchEvent(new OpenPopupPayload("Resources",
                    "The single most important resource your civilization needs to survive is food, grown by Farmers.\n" +
                    "A second critical resource are tools. Tools are provided by craftsmen"));
            }));

        framework = new IdleEngine(configurationBuilder.Build(), gameObject);
        Logger.globalLevel = Logger.Level.TRACE;
        InvokeRepeating("tick", 0f, 1f);
    }

    void tick()
    {
        if (!paused)
        {
            framework.Update(1f);
        }
    }

    void openPopup(OpenPopupPayload payload)
    {
        paused = true;
        this.BroadcastMessage("UpdateModal",
            new Modal.ModalValues(payload.Title, payload.Body, true, this.modalClosed));
    }

    void modalClosed()
    {
        paused = false;
    }

}
