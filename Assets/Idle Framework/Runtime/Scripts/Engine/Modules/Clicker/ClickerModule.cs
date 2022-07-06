using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.idle.framework.Modifiers;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
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

        public void ConfigureEngine(IdleEngine engine)
        {
            engine.RegisterMethod("PointsUpdater", PointsUpdater);
            engine.RegisterMethod("ProducerUpdater", (ctx, args) => Updater<ProducerDefinition>("producer", ctx, args));
            engine.RegisterMethod("UpgradeUpdater", (ctx, args) => Updater<UpgradeDefinition>("upgrade", ctx, args));
            engine.RegisterMethod("DoClick", (ctx, args) =>
            {
                DoClick(ctx, args);
                return DynValue.Nil;
            });
            engine.CreateProperty(DefaultProperties.PRODUCER_COST_SCALE_FACTOR, 1.15);
            engine.SetDefinitions("producer", definitions["producer"]);
            engine.CreateProperty("points.income", 0);
            engine.CreateProperty("points.click_income", 1);
            engine.CreateProperty("points.quantity", 0, updater: "return value + parent.income");
            foreach (var producer in definitions["producer"])
            {
                string producerBasePath = string.Join(".", "producers", producer.Key);
                ValueContainer producerContainer = engine.CreateProperty(producerBasePath, new Dictionary<string, ValueContainer>(), updater: "return ProducerUpdater(container)");
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.QUANTITY), 0)
                    .Subscribe(String.Format("producer {0} quantity", producer.Key), ValueChangedEvent.EventName, string.Format("producers.{0}.ForceUpdate()", producer.Key));
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.COST), ((ProducerDefinition)producer.Value).CostExpression, updater: string.Format("return {0} * {1} ^ parent.quantity", ((ProducerDefinition)producer.Value).CostExpression, DefaultProperties.PRODUCER_COST_SCALE_FACTOR));
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.OUTPUT_PER_UNIT), ((ProducerDefinition)producer.Value).OutputPerSecond)
                    .Subscribe(String.Format("producer {0} output", producer.Key), ValueChangedEvent.EventName, string.Format("producers.{0}.ForceUpdate()", producer.Key)); ;
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.TOTAL_OUTPUT), 0, updater: "return parent.quantity * parent.output_per_unit_per_second");
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.ENABLED), false);
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.UNLOCKED), false);
                engine.CreateProperty(string.Join(".", producerBasePath, ProducerDefinition.PropertyNames.BUYABLE), false);

                string producerOutput = String.Format("value + producers.{0}.total_output", producer.Key);
                engine.GetProperty("points.income").AddModifier(
                    new ValueModifier("producer " + producer.Key, "producer " + producer.Key, "return " + producerOutput, engine)
                    );
            }

            foreach (var upgrade in definitions["upgrade"])
            {
                string upgradeBasePath = string.Join(".", "upgrades", upgrade.Key);
                engine.CreateProperty(upgradeBasePath, updater: "return UpgradeUpdater(container)");
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.COST), ((UpgradeDefinition)upgrade.Value).CostExpression);
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.ENABLED), false);
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.UNLOCKED), false);
                engine.CreateProperty(string.Join(".", upgradeBasePath, ProducerDefinition.PropertyNames.BUYABLE), false);
            }

            engine.RegisterMethod("BuyUpgrade", BuyUpgrade);
            engine.RegisterMethod("BuyProducer", BuyProducer);
        }

        private static void DoClick(ScriptExecutionContext ctx, CallbackArguments args)
        {
            IdleEngine engine = ctx.CurrentGlobalEnv["engine"] as IdleEngine;
            BigDouble currentPoints = engine.GetProperty("points.quantity").AsNumber;
            BigDouble clickIncome = engine.GetProperty("points.click_income").AsNumber;
            engine.GetProperty("points.quantity").Set(currentPoints + clickIncome);
        }

        private static DynValue Updater<T>(string type, ScriptExecutionContext ctx, CallbackArguments args) where T : IDefinition
        {
            IdleEngine engine = ctx.CurrentGlobalEnv["engine"] as IdleEngine;
            var container = args[0].ToObject<ValueContainer>();

            T entityDef = engine.GetDefinition<T>(type, container.Path.Substring(container.Path.LastIndexOf(".") + 1));
            if (!container[ProducerDefinition.PropertyNames.ENABLED].ValueAsBool() && ((IEnableable)entityDef).EnableExpression != null)
            {
                container[ProducerDefinition.PropertyNames.ENABLED].Set((bool)engine.EvaluateExpression("return " + ((IEnableable)entityDef).EnableExpression));
                if (container[ProducerDefinition.PropertyNames.ENABLED])
                {
                    //engine.Broadcast(String.Format("{0}_enabled", type), entityDef);
                    engine.Broadcast(String.Format("{0}_enabled", type), null, new DefinitionEnabledEvent());
                }
            }
            if (!container[ProducerDefinition.PropertyNames.UNLOCKED].ValueAsBool() && ((IUnlockable)entityDef).UnlockExpression != null)
            {
                container[ProducerDefinition.PropertyNames.UNLOCKED].Set((bool)engine.EvaluateExpression("return " + ((IUnlockable)entityDef).UnlockExpression));
                if (container[ProducerDefinition.PropertyNames.UNLOCKED])
                {
                    engine.Broadcast(String.Format("{0}_unlocked", type), null, new DefinitionUnlockedEvent());
                }
            }
            BigDouble quantity = container.ValueAsMap().ContainsKey(ProducerDefinition.PropertyNames.QUANTITY) ? container[ProducerDefinition.PropertyNames.QUANTITY].ValueAsNumber() : 0;
            container.GetProperty(ProducerDefinition.PropertyNames.BUYABLE).Set(CalculatePurchaseCost(engine, (IBuyable)entityDef, quantity) <= engine.GetProperty("points.quantity").ValueAsNumber());
            //return args[2];
            return DynValue.FromObject(ctx.GetScript(), container.AsMap);
        }

        private static DynValue PointsUpdater(ScriptExecutionContext ctx, CallbackArguments args)
        {
            var engine = args[0].ToObject<IdleEngine>();
            var updateTime = args[1].ToObject<BigDouble>();
            BigDouble previousValue = args[2].ToObject<BigDouble>();
            return DynValue.FromObject(ctx.GetScript(), previousValue + (engine.GetProperty("points.income").ValueAsNumber() * updateTime));
        }

        public static void SpendPoints(IdleEngine engine, BigDouble quantity)
        {
            var points = engine.GetProperty("points.quantity");
            points.Set(points.ValueAsNumber() - quantity);
        }

        public static DynValue BuyProducer(ScriptExecutionContext ctx, CallbackArguments args)
        {
            IdleEngine engine = ctx.CurrentGlobalEnv["engine"] as IdleEngine;
            string producerId = args[0].CastToString();
            ProducerDefinition producerDefinition = engine.GetDefinition<ProducerDefinition>("producer", producerId);
            ValueContainer points = engine.GetProperty("points.quantity");
            SpendPoints(engine, CalculatePurchaseCost(engine, producerDefinition, engine.GetProperty("producers." + producerId + ".quantity").ValueAsNumber()));
            engine.GetProperty(String.Format("producers.{0}.quantity", producerId)).Set(engine.GetProperty(String.Format("producers.{0}.quantity", producerId)).ValueAsNumber() + 1);
            return null;
        }

        public static BigDouble CalculatePurchaseCost(IdleEngine engine, IBuyable buyable, BigDouble quantity)
        {
            return ((BigDouble)engine.EvaluateExpression("return " + buyable.CostExpression) * new BigDouble(1.15).Pow(quantity)).Ceiling();
        }

        public static DynValue BuyUpgrade(ScriptExecutionContext ctx, CallbackArguments args)
        {
            IdleEngine engine = ctx.CurrentGlobalEnv["engine"] as IdleEngine;
            string upgradeId = args[0].CastToString();
            UpgradeDefinition upgradeDefinition = engine.GetDefinition<UpgradeDefinition>("upgrade", upgradeId);
            foreach (var modifier in upgradeDefinition.GenerateModifiers(engine))
            {
                engine.GetProperty(modifier.Key)
                    .AddModifier(modifier.Value);
            }
            SpendPoints(engine, CalculatePurchaseCost(engine, upgradeDefinition, 0));
            engine.GetProperty(String.Join(".", "upgrades", upgradeId)).NotifyImmediately("upgrade_bought", new UpgradeBoughtEvent(upgradeId));
            return null;
        }

        public void AssertReady()
        {

        }

        public static class DefaultProperties
        {
            public const string PRODUCER_COST_SCALE_FACTOR = "PRODUCER_COST_SCALE_FACTOR";
        }
    }
}