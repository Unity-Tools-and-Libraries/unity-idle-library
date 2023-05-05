using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using static io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Producer.PropertyNames;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerModule : IModule
    {
        private Dictionary<double, Producer> producers = new Dictionary<double, Producer>();
        private Dictionary<double, Upgrade> upgrades = new Dictionary<double, Upgrade>();

        public void AddUpgrade(Upgrade upgrade)
        {
            if(upgrades.ContainsKey(upgrade.Id))
            {
                throw new Exception("Already contains an upgrade with id " + upgrade.Id);
            }
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
            UserData.RegisterType<ResourceHolder>();
            UserData.RegisterExtensionType(typeof(ClickerEngineExtensionMethods));
            UserData.RegisterType<ProducerInstance>();
            UserData.RegisterType<UpgradeInstance>();

            engine.Scripting.AddTypeAdaptor(new scripting.types.TypeAdapter<IDictionary<long, Producer>>.AdapterBuilder()
                .WithClrConversion(DictionaryTypeAdapter.Converter)
                .Build());

            engine.GetDefinitions()["producers"] = new Dictionary<double, Producer>();
            engine.GetDefinitions()["upgrades"] = new Dictionary<double, Upgrade>();

            
            foreach (var producer in producers)
            {
                engine.GetProducers()[producer.Key] =  producer.Value;
            }

            foreach(var upgrade in upgrades)
            {
                engine.GetUpgrades()[upgrade.Key] = upgrade.Value;
            }

            engine.GlobalProperties["player"] = new ClickerPlayer(engine, 0) {
                Resources = new Dictionary<string, ResourceHolder>()
                {
                    { "points", new ResourceHolder() }
                }
            }.Initialize();
        }

        public void AssertReady()
        {

        }

        public void Configure(IdleEngine engine)
        {
            SetDefinitions(engine);
            SetGlobalProperties(engine);
        }

        public void SetDefinitions(IdleEngine engine)
        {
            
        }

        public string[] GetPreinitializationScriptPaths()
        {
            return new string[0];
        }

        public static class DefaultProperties
        {
            public const string PRODUCER_COST_SCALE_FACTOR = "PRODUCER_COST_SCALE_FACTOR";
        }

        public static class States
        {
            public const string GAMEPLAY = "gameplay";
        }
    }

    public static class ClickerEngineExtensionMethods
    {
        public static void DoClick(this IdleEngine engine, string resourceToClick)
        {
            engine.Scripting.EvaluateStringAsScript("globals.player.GetResource(resourceName).quantity = globals.player.GetResource(resourceName).quantity + globals.player.GetResource(resourceName).click_income",
                Tuple.Create<string, object>("resourceName", resourceToClick));
        }

        public static IDictionary<double, Producer> GetProducers(this IdleEngine engine)
        {
            return engine.GetDefinitions()["producers"] as IDictionary<double, Producer>;
        }

        public static Producer GetProducer(this Player player, string producerId)
        {
            return null;
        }

        public static IDictionary<double, Upgrade> GetUpgrades(this IdleEngine engine)
        {
            return engine.GetDefinitions()["upgrades"] as IDictionary<double, Upgrade>;
        }

        public static ClickerPlayer GetPlayer(this IdleEngine engine)
        {
            return engine.GlobalProperties["player"] as ClickerPlayer;
        }

        public static Dictionary<string, BigDouble> CalculatePurchaseCost(this IdleEngine engine, IBuyable buyable, BigDouble alreadyOwnedQuantity, BigDouble toBuyQuantity)
        {
            return buyable.CostExpressions.ToDictionary(x => x.Key, x =>
            {
                BigDouble total = 0;
                BigDouble nextQuantity = alreadyOwnedQuantity;
                while (nextQuantity < toBuyQuantity + alreadyOwnedQuantity)
                {
                    total += engine.Scripting.EvaluateStringAsScript(x.Value, Tuple.Create<string, object>("level", alreadyOwnedQuantity)).ToObject<BigDouble>();
                    nextQuantity++;
                }
                return total;
            });
        }
    }


}