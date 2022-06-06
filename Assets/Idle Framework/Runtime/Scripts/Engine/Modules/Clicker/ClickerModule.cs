using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static io.github.thisisnozaku.idle.framework.ValueContainer;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerModule : IModule
    {
        private Dictionary<string, IDictionary<string, IDefinition>> definitions = new Dictionary<string, IDictionary<string, IDefinition>>()
        {
            { "resource", new Dictionary<string, IDefinition>()
                {
                    { "points", new ResourceDefinition("points", "points", null) }
                }
            },
            { "producer", new Dictionary<string, IDefinition>()
                {
                }
            },
            { "upgrade", new Dictionary<string, IDefinition>()
                {
                } 
            }
        };

        public void AddUpgrade(UpgradeDefinition upgrade)
        {
            if (definitions["upgrade"].ContainsKey(upgrade.Id) && definitions["upgrade"][upgrade.Id] != upgrade)
            {
                throw new ArgumentException("Duplicate ID " + upgrade.Id + " used.");
            }
            definitions["upgrade"][upgrade.Id] = upgrade;
        }

        public void AddProducer(ProducerDefinition producer)
        {
            if (definitions["producer"].ContainsKey(producer.Id) && definitions["producer"][producer.Id] != producer)
            {
                throw new ArgumentException("Duplicate ID " + producer.Id + " used.");
            }
            definitions["producer"][producer.Id] = producer;
        }

        public IDictionary<string, IDictionary<string, IDefinition>> GetDefinitions()
        {
            return definitions;
        }

        public void SetEngineProperties(IdleEngine engine)
        {
            engine.RegisterMethod(PointsUpdater);
            engine.SetProperty(DefaultProperties.PRODUCER_COST_SCALE_FACTOR, 1.15);
            engine.SetDefinitions("producer", definitions["producer"]);
            engine.SetProperty("points.income", 0);
            engine.SetProperty("points.quantity", 0, updater: "PointsUpdater");
            foreach (var producer in definitions["producer"])
            {
                string producerPath = string.Join(".", "producers", producer.Key);
                engine.SetProperty(producerPath, 0);
                engine.SetProperty(string.Join(".", producerPath, ProducerDefinition.PropertyNames.COST), ((ProducerDefinition)producer.Value).BaseCost, modifiers: new List<ContainerModifier>() {
                    new MultiplicativeValueModifier("1", "cost multiplier", "1.15 ^ this.quantity", ValueContainer.Context.ParentGenerator)
                });
                engine.SetProperty(string.Join(".", producerPath, ProducerDefinition.PropertyNames.QUANTITY), 0);
                engine.SetProperty(string.Join(".", producerPath, ProducerDefinition.PropertyNames.OUTPUT_PER_UNIT), ((ProducerDefinition)producer.Value).OutputPerSecond);
                engine.SetProperty(string.Join(".", producerPath, ProducerDefinition.PropertyNames.TOTAL_OUTPUT), 0, modifiers: new List<ContainerModifier>()
                {
                    new AdditiveValueModifier("base", "base", "this.output_per_unit_per_second * this.quantity", contextGenerator: Context.ParentGenerator)
                });

                engine.GetProperty("points.income").AddModifier(
                    new AdditiveValueModifier("producer " + producer.Key, "producer " + producer.Key, String.Format("producers.{0}.total_output", producer.Key), ValueContainer.Context.GlobalContextGenerator)
                    );
            }

            engine.RegisterMethod(BuyUpgrade);
        }

        private static object PointsUpdater(IdleEngine engine, ValueContainer container, params object[] args)
        {
            var updateEvent = (args[0] as ValueContainerWillUpdateEvent);
            return (BigDouble)updateEvent.PreviousValue + (engine.GetProperty("points.income").ValueAsNumber() * updateEvent.Time);
        }

        public static object BuyUpgrade(IdleEngine engine, ValueContainer container, params object[] args)
        {
            string upgradeId = args[0] as string;
            UpgradeDefinition upgradeDefinition = engine.GetDefinition<UpgradeDefinition>("upgrade", upgradeId);
            foreach(var target in upgradeDefinition.UpgradeTargets)
            {
                engine.GetProperty(String.Join(".", "producers", target, ProducerDefinition.PropertyNames.OUTPUT_PER_UNIT))
                    .AddModifier(upgradeDefinition.GenerateModifier());
            }
            return null;
        }

        public static class DefaultProperties
        {
            public const string PRODUCER_COST_SCALE_FACTOR = "PRODUCER_COST_SCALE_FACTOR";
        }
    }
}