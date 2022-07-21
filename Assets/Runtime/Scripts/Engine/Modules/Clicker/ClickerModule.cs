using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;
using System;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Producer.PropertyNames;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerModule : IModule
    {
        private Dictionary<long, Producer> producers = new Dictionary<long, Producer>();
        private Dictionary<long, Upgrade> upgrades = new Dictionary<long, Upgrade>();

        public void AddUpgrade(Upgrade upgrade)
        {
            upgrades[upgrade.Id] = upgrade;
        }

        public void AddProducer(Producer producer)
        {
            producers[producer.Id] = producer;
        }

        public void SetGlobalProperties(IdleEngine engine)
        {
            UserData.RegisterType<ClickerPlayer>();
            UserData.RegisterType<Producer>();
            UserData.RegisterType<Upgrade>();
            UserData.RegisterType<PointsHolder>();

            engine.GetDefinitions()["producers"] = new Dictionary<long, Producer>();
            engine.GetDefinitions()["upgrades"] = new Dictionary<long, Upgrade>();
            engine.GlobalProperties["DoClick"] = (Action)(() => DoClick(engine));

            //engine.GlobalProperties.Set(DefaultProperties.PRODUCER_COST_SCALE_FACTOR] DynValue.FromObject(null, 1.15));


            foreach (var producer in producers)
            {
                engine.GetProducers()[producer.Key] =  producer.Value;
            }

            foreach(var upgrade in upgrades)
            {
                engine.GetUpgrades()[upgrade.Key] = upgrade.Value;
            }

            engine.GlobalProperties["player"] = new ClickerPlayer(engine, 0);
        }

        private static void DoClick(IdleEngine engine)
        {
            engine.Scripting.Evaluate("player.points.quantity = player.points.quantity + player.points.click_income");
        }

        public void AssertReady()
        {

        }

        public void SetConfiguration(IdleEngine engine)
        {
            
        }

        public void SetDefinitions(IdleEngine engine)
        {
            
        }

        public static class DefaultProperties
        {
            public const string PRODUCER_COST_SCALE_FACTOR = "PRODUCER_COST_SCALE_FACTOR";
        }
    }

    public static class ClickerEngineExtensionMethods
    {
        public static IDictionary<long, Producer> GetProducers(this IdleEngine engine)
        {
            return engine.GetDefinitions()["producers"] as IDictionary<long, Producer>;
        }

        public static IDictionary<long, Upgrade> GetUpgrades(this IdleEngine engine)
        {
            return engine.GetDefinitions()["upgrades"] as IDictionary<long, Upgrade>;
        }

        public static ClickerPlayer GetPlayer(this IdleEngine engine)
        {
            return engine.GlobalProperties["player"] as ClickerPlayer;
        }

        public static BigDouble CalculatePurchaseCost(this IdleEngine engine, IBuyable buyable, BigDouble alreadyOwnedQuantity)
        {
            return (engine.Scripting.Evaluate(buyable.CostExpression).ToObject<BigDouble>() * new BigDouble(1.15).Pow(alreadyOwnedQuantity - 1)).Ceiling();
        }

    }
}