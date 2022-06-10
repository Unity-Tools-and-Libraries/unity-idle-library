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
            engine.RegisterMethod(DoClick);
            engine.RegisterMethod("ProducerUpdater", (ie, vc, args) => Updater<ProducerDefinition>("producer", ie, vc, args));
            engine.RegisterMethod("UpgradeUpdater", (ie, vc, args) => Updater<UpgradeDefinition>("upgrade", ie, vc, args));

            engine.CreateProperty(DefaultProperties.PRODUCER_COST_SCALE_FACTOR, 1.15);
            engine.SetDefinitions("producer", definitions["producer"]);
            engine.CreateProperty("points.income", 0);
            engine.CreateProperty("points.click_income", 1);
            engine.CreateProperty("points.quantity", 0, updater: "PointsUpdater");
            foreach (var producer in definitions["producer"])
            {
                string producerBasePath = string.Join(".", "producers", producer.Key);
                engine.CreateProperty(producerBasePath, updater: "ProducerUpdater");
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.COST), ((ProducerDefinition)producer.Value).CostExpression, modifiers: new List<IContainerModifier>() {
                    new MultiplicativeValueModifier(String.Format("producer {0} quantity cost modifier", producer.Key), "cost multiplier", "1.15 ^ this.quantity", new string[] { producerBasePath + ".quantity" }, ValueContainer.Context.ParentGenerator)
                });
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.QUANTITY), 0);
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.OUTPUT_PER_UNIT), ((ProducerDefinition)producer.Value).OutputPerSecond);
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.OUTPUT_MULTIPLIER), 1);
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.TOTAL_OUTPUT), 0, modifiers: new List<IContainerModifier>()
                {
                    new AdditiveValueModifier(producer.Key + "base", "base", "this.output_per_unit_per_second * this.quantity * this.output_multiplier",
                    new string[] { "this.output_per_unit_per_second", "this.quantity", "this.output_multiplier"},
                    contextGenerator: Context.ParentGenerator)
                });
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.ENABLED), false);
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.UNLOCKED), false);

                string producerOutput = String.Format("producers.{0}.total_output", producer.Key);
                engine.GetProperty("points.income").AddModifier(
                    new AdditiveValueModifier("producer " + producer.Key, "producer " + producer.Key, producerOutput, new string[] { producerOutput }, ValueContainer.Context.GlobalContextGenerator)
                    );
            }

            foreach (var upgrade in definitions["upgrade"])
            {
                string upgradeBasePath = string.Join(".", "upgrades", upgrade.Key);
                engine.CreateProperty(upgradeBasePath, updater: "UpgradeUpdater");
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.COST), ((UpgradeDefinition)upgrade.Value).CostExpression);
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.ENABLED), false);
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.UNLOCKED), false);
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.BUYABLE), false);
            }

            engine.RegisterMethod(BuyUpgrade);
            engine.RegisterMethod(BuyProducer);
        }

        private static object Updater<T>(string type, IdleEngine engine, ValueContainer container, params object[] args)  where T: IDefinition
        {
            T entityDef = engine.GetDefinition<T>(type, container.Path.Substring(container.Path.LastIndexOf(".") + 1));
            if (!container[ProducerDefinition.PropertyNames.ENABLED].ValueAsBool() && ((IEnableable)entityDef).EnableExpression != null)
            {
                container[ProducerDefinition.PropertyNames.ENABLED].Set((bool)engine.EvaluateExpression(((IEnableable)entityDef).EnableExpression));
                if (container[ProducerDefinition.PropertyNames.ENABLED])
                {
                    engine.Broadcast(String.Format("{0}_enabled", type), entityDef);
                }
            }
            if (!container[ProducerDefinition.PropertyNames.UNLOCKED].ValueAsBool() && ((IUnlockable)entityDef).UnlockExpression != null)
            {
                container[ProducerDefinition.PropertyNames.UNLOCKED].Set((bool)engine.EvaluateExpression(((IUnlockable)entityDef).UnlockExpression));
                if (container[ProducerDefinition.PropertyNames.UNLOCKED])
                {
                    engine.Broadcast(String.Format("{0}_unlocked", type), entityDef);
                }
            }
            BigDouble quantity = container.ValueAsMap().ContainsKey(ProducerDefinition.PropertyNames.QUANTITY) ? container[ProducerDefinition.PropertyNames.QUANTITY].ValueAsNumber() : 0;
            container[ProducerDefinition.PropertyNames.BUYABLE].Set(CalculatePurchaseCost(engine, (IBuyable)entityDef, quantity) <= engine.GetProperty("points.quantity").ValueAsNumber());
            return args[1];
        }

        private static object DoClick(IdleEngine engine, ValueContainer container, object[] args)
        {
            BigDouble currentPoint = engine.GetProperty("points.quantity").ValueAsNumber();
            engine.GetProperty("points.quantity").Set(currentPoint + engine.GetProperty("points.click_income").ValueAsNumber());
            return null;
        }

        private static object PointsUpdater(IdleEngine engine, ValueContainer container, params object[] args)
        {
            var updateTime = (float)args[0];
            BigDouble previousValue = (BigDouble)args[1];
            return previousValue + (engine.GetProperty("points.income").ValueAsNumber() * updateTime);
        }

        public static void SpendPoints(IdleEngine engine, BigDouble quantity)
        {
            var points = engine.GetProperty("points.quantity");
            points.Set(points.ValueAsNumber() - quantity);
        }

        public static object BuyProducer(IdleEngine engine, ValueContainer container, object[] args)
        {
            string producerId = args[0] as string;
            ProducerDefinition producerDefinition = engine.GetDefinition<ProducerDefinition>("producer", producerId);
            ValueContainer points = engine.GetProperty("points.quantity");
            SpendPoints(engine, CalculatePurchaseCost(engine, producerDefinition, engine.GetProperty("producers." + producerId + ".quantity").ValueAsNumber()));
            engine.GetProperty(String.Format("producers.{0}.quantity", producerId)).Set(engine.GetProperty(String.Format("producers.{0}.quantity", producerId)).ValueAsNumber() + 1);
            return null;
        }

        public static BigDouble CalculatePurchaseCost(IdleEngine engine, IBuyable buyable, BigDouble quantity)
        {
            return ((BigDouble)engine.EvaluateExpression(buyable.CostExpression) * new BigDouble(1.15).Pow(quantity)).Ceiling();
        }

        public static object BuyUpgrade(IdleEngine engine, ValueContainer container, params object[] args)
        {
            string upgradeId = args[0] as string;
            UpgradeDefinition upgradeDefinition = engine.GetDefinition<UpgradeDefinition>("upgrade", upgradeId);
            foreach(var modifier in upgradeDefinition.GenerateModifiers())
            {
                engine.GetProperty(modifier.Key)
                    .AddModifier(modifier.Value);
            }
            SpendPoints(engine, CalculatePurchaseCost(engine, upgradeDefinition, 0));
            engine.GetProperty(String.Join(".", "upgrades", upgradeId)).NotifyImmediately("upgrade_bought", upgradeId);
            return null;
        }

        public static class DefaultProperties
        {
            public const string PRODUCER_COST_SCALE_FACTOR = "PRODUCER_COST_SCALE_FACTOR";
        }
    }
}