using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Producer.PropertyNames;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerModule : IModule
    {
        private Dictionary<string, Producer> producers = new Dictionary<string, Producer>();
        private Dictionary<string, Upgrade> upgrades = new Dictionary<string, Upgrade>();

        public void AddUpgrade(Upgrade upgrade)
        {
            upgrades[upgrade.Id] = upgrade;
        }

        public void AddProducer(Producer producer)
        {
            producers[producer.Id] = producer;
        }

        public void ConfigureEngine(IdleEngine engine)
        {
            UserData.RegisterType<Producer>();

            engine.GlobalProperties["producers"] = new Dictionary<string, Producer>();
            engine.GlobalProperties["DoClick"] = (Action)(() => DoClick(engine));
            
            //engine.GlobalProperties.Set(DefaultProperties.PRODUCER_COST_SCALE_FACTOR] DynValue.FromObject(null, 1.15));
            
            engine.GlobalProperties["points"] = new Dictionary<string, object>()
            {
                { "income", 0 },
                { "click_income", 1 },
                { "quantity", 0 }
            };


            foreach (var producer in producers)
            {
                engine.GetProducers()[producer.Key] =  producer.Value;
            }

            //engine.RegisterMethod("BuyUpgrade", BuyUpgrade);
            engine.GlobalProperties["BuyProducer"] = (Action<string>)((producer) => BuyProducer(engine, producer));
        }

        private static void DoClick(IdleEngine engine)
        {
            engine.Scripting.Evaluate("points.quantity = points.quantity + points.click_income");
        }

        public static void BuyProducer(IdleEngine engine, string producerId)
        {
            Producer producerDefinition = engine.GetProducers()[producerId];
            BigDouble points = engine.GetPoints();
            engine.ChangePoints(CalculatePurchaseCost(engine, producerDefinition, engine.GetProducers()[producerId].Quantity));
            producerDefinition.Quantity += 1;

        }

        public static BigDouble CalculatePurchaseCost(IdleEngine engine, IBuyable buyable, BigDouble quantity)
        {
            return (engine.Scripting.Evaluate("return " + buyable.CostExpression).ToObject<BigDouble>() * new BigDouble(1.15).Pow(quantity)).Ceiling();
        }

        public static DynValue BuyUpgrade(ScriptExecutionContext ctx, CallbackArguments args)
        {
            IdleEngine engine = ctx.CurrentGlobalEnv["engine"] as IdleEngine;
            string upgradeId = args[0].CastToString();
            //Upgrade upgradeDefinition = engine.GlobalProperties["upgrades"].ToObject<Upgrade>();
            //foreach (var modifier in upgradeDefinition.GenerateModifiers(engine))
            {
                //engine.GlobalProperties[modifier.Key]
                    //.AddModifier(modifier.Value);
            }
            //SpendPoints(engine, CalculatePurchaseCost(engine, upgradeDefinition, 0));
            //engine.GlobalProperties[String.Join(".", "upgrades", upgradeId]).NotifyImmediately("upgrade_bought", new UpgradeBoughtEvent(upgradeId));
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

    public static class ClickerEngineExtensionMethods
    {
        public static BigDouble GetPoints(this IdleEngine engine)
        {
            return engine.GetProperty<BigDouble>("points.quantity");
        }

        public static void SetPoints(this IdleEngine engine, BigDouble value)
        {
            engine.GlobalProperties["points"] = value;
        }

        public static void ChangePoints(this IdleEngine engine, BigDouble quantityChange)
        {
            engine.SetPoints(engine.GetPoints() + quantityChange);
        }

        public static IDictionary<string, Producer> GetProducers(this IdleEngine engine)
        {
            return engine.GlobalProperties["producers"] as IDictionary<string, Producer>;
        }

    }
}