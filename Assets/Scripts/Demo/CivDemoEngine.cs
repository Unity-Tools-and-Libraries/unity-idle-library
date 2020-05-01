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

    void Awake()
    {
        var configurationBuilder = new GameConfigurationBuilder();

        configurationBuilder.WithCustomGlobalProperty("government", Literal.Of("citystate"));

        configurationBuilder.WithBeforeUpdateHook((engine, deltaTime) => {
            // Determine job prestige for apportionment of resources.
            var jobPrestige = new Dictionary<string, BigDouble>();
        });

        configurationBuilder.WithBeforeUpdateHook((engine, deltaTime) =>
        {
            BigDouble population = BigDouble.Floor(engine.GetEntity("population").Quantity);
            string governmentName = engine.GetGlobalStringProperty("government").Get(engine);
            Entity government = engine.GetEntity("government").GetVariant(governmentName);
            var jobWeights = government.GetMapProperty("jobs");
            IList<string> jobsSortedByWeight = new List<string>(
                jobWeights.AsEnumerable()
                    .OrderBy(e => e.Value.AsNumber().Get(engine), Comparer<BigDouble>.Create((BigDouble a, BigDouble b) => {
                        var compareResult = b.CompareTo(a);
                        return compareResult;
                    }))
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
                if (accumulatedRemainder <= 0)
                {
                    break;
                }
                assignedPopulation[job] = assignedPopulation[job] + 1;
                accumulatedRemainder -= 1;
                
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
            .WithScaledOutput("population", 
                Min.Of(Literal.Of(.01), new EntityNumberPropertyReference("population", "Quantity").Times(Literal.Of(0.01))))
            );

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("farmer")
            .WithType("job")
            .WithScaledOutput("food", Literal.Of(2)
                .Plus(
                    Min.Of(
                        new EntityNumberPropertyReference("farmer", "Quantity"), 
                        new EntityNumberPropertyReference("tools", "Quantity")
                        )
                    )
                )
            );

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("warrior")
            .WithType("job")
            .WithUpkeepRequirement("weapons", 1));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("craftsman")
            .WithType("job")
            .WithScaledOutput("tools", 1)
            .WithScaledOutput("weapons", 1));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("food")
            .WithType("resource")
            .DoesNotAccumulate());

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("advancement")
            .WithType("resource"));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("government")
            .WithCustomMapProperty("jobs", new Dictionary<string, ValueContainer>()
            {
                { "farmer",     Literal.Of(1) },
                { "craftsman",  Literal.Of(1) },
                { "warrior",    Literal.Of(1) }
            })
            .WithVariant(
                new EntityDefinitionBuilder("citystate")
                .WithCustomMapProperty("jobs", new Dictionary<string, ValueContainer>()
                {
                    { "craftsman",  Literal.Of(2) },
                    { "farmer",     Literal.Of(3) },
                    { "warrior",    Literal.Of(1) }
                })
            ));

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("tools")
            .WithType("resource")
            .DoesNotAccumulate());

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder("intro")
            .InitiatesWhen(Always.Instance)
            .ExecutesHookOnInitiation(engine =>
            {
                engine.Events.DispatchEvent(new OpenPopupPayload("Welcome", "You are seeking to lead a civilization to conquer the world... And beyond!\n" +
                    "The basis of your civilization is your population. These are the people that do the work of growing the food, constructing the buildings, manufacturing the tools and fighting the battles.\n" +
                    "Jobs are the tasks that your population does. In these early years, farmers grow food, craftsmen manufacture tools and weapons and soldiers fight battles."));
            }).CompletesWhen(Always.Instance));

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder("resources")
            .InitiatesWhen(new TutorialCompleteMatcher("intro"))
            .ExecutesHookOnCompletion(engine => {
                engine.Events.DispatchEvent(new OpenPopupPayload("Resources",
                    "The single most important resource your civilization needs to survive is food, grown by Farmers.\n" +
                    "A second critical resource are tools. Tools are provided by craftsmen\n" +
                    "After that are weapons, which your warriors use in battle.\n" +
                    "As your civilization advances, new resources will become available for use."));
            }));

        configurationBuilder.WithTutorial(new TutorialConfigurationBuilder("development")
            .InitiatesWhen(new TutorialCompleteMatcher("resources"))
            .ExecutesHookOnCompletion(engine => {
                engine.Events.DispatchEvent(new OpenPopupPayload("Development",
                    ""));
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
