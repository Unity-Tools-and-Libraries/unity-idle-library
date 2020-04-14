using IdleFramework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Weighted_Randomizer;
using BreakInfinity;

public class ShadowSpiritsDemoEngine : MonoBehaviour
{
    public IdleFramework.IdleEngine framework;
    private float tickRate = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        GameConfigurationBuilder configurationBuilder = new GameConfigurationBuilder();

        configurationBuilder.WithCustomGlobalProperty("playerMode", Literal.Of("exploration"));
        configurationBuilder.WithCustomGlobalProperty("activeEncounter", Literal.Of(""));

        configurationBuilder.WithSingletonEntity(new EntityDefinitionBuilder("character")
            .WithCustomMapProperty("attributes")
            .WithVariant(new EntityDefinitionBuilder("player"))
            .WithVariant(new EntityDefinitionBuilder("human skeleton"))
            );

        configurationBuilder.WithEntity(new EntityDefinitionBuilder("encounter")
            .WithCustomStringProperty("type")
            .WithCustomNumberProperty("weight")
            .WithVariant(new EntityDefinitionBuilder("fight"))
            );

        IWeightedRandomizer<string> encounterTable = new DynamicWeightedRandomizer<string>();

        configurationBuilder.WithStartupHook(engine =>
        {
            foreach (var encounter in engine.GetEntity("encounter").Variants.Values)
            {
                int weight = (int)encounter.GetNumberProperty("weight").ToDouble();
                encounterTable.Add(encounter.VariantKey, weight);
            }
        });

        float randomEncounterChance = .1f;

        configurationBuilder.WithEventHook("encounterStart", (engine, arg) =>
        {
            string nextEncounter = encounterTable.NextWithReplacement();
            Entity encounter = engine.GetEntity("encounter").GetVariant(nextEncounter);
            MapLiteral activeEncounter = Literal.Of(new Dictionary<string, ValueContainer>(){
                { "encounterId", Literal.Of(encounter.VariantKey)},
                { "participants", Literal.Containing(
                    Literal.Of(new Dictionary<string, ValueContainer>(){
                        { "id", Literal.Of("player") },
                        { "actionGauge", Literal.Of(0) }
                    }))
                }
            });

            ListLiteral participants = Literal.Containing();

            engine.SetGlobalProperty("activeEncounter", activeEncounter);
            switch (encounter.GetStringProperty("type"))
            {
                case "combat":
                    engine.Log("Beginning combat encounter.", Logger.Level.TRACE);
                    engine.SetGlobalProperty("playerMode", "combat");
                    // Add enemy to the fight.
                    participants.Add(Literal.Of(new Dictionary<string, ValueContainer>() {
                        { "id", Literal.Of("skeleton") }
                    }));
                    break;

            }
        });

        Func<ListContainer, IList<string>> actionSelector = (participants) =>
        {
            List<string> actions = new List<string>();
            // Determine if actions
            foreach (var participant in participants)
            {

            }
            return actions;
        };

        configurationBuilder.WithUpdateHook((IdleEngine engine, float deltaTime) =>
        {
            var mode = engine.GetGlobalStringProperty("playerMode").Get(engine);
            MapContainer activeEncounter = engine.GetGlobalMapProperty("activeEncounter");
            switch (mode)
            {
                case "exploration":
                    float randomEncounterOccuranceCheck = UnityEngine.Random.value;
                    bool randomEncounterOccured = randomEncounterChance.CompareTo(randomEncounterOccuranceCheck) != 1;
                    if (randomEncounterOccured)
                    {
                        engine.Log("random encounter triggered", Logger.Level.TRACE);
                        engine.Events.DispatchEvent("encounterStart");
                    }
                    break;
                case "combat":
                    ListContainer participants = activeEncounter.Get(engine)["particpants"].AsList();
                    foreach (var participant in participants)
                    {
                        BigDouble currentActionGauge = participant.AsMap().Get(engine)["actionGauge"].AsNumber().Get(engine);
                        participant.AsMap().Set("actionGague", Literal.Of(currentActionGauge + deltaTime));
                    }
                    IList<string> participantActions = actionSelector(participants);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        });

        framework = new IdleEngine(configurationBuilder.Build(), gameObject);
        Logger.globalLevel = Logger.Level.TRACE;
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
