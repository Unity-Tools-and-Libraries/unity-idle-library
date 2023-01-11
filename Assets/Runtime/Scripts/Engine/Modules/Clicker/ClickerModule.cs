using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using io.github.thisisnozaku.idle.framework.Engine.State;
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
            UserData.RegisterType<ResourceHolder>();

            engine.Scripting.AddTypeAdaptor(new scripting.types.TypeAdapter<IDictionary<long, Producer>>.AdapterBuilder<IDictionary<long, Producer>>()
                .WithClrConversion(DictionaryTypeAdapter.Converter)
                .Build());


            engine.GetDefinitions()["producers"] = new Dictionary<long, Producer>();
            engine.GetDefinitions()["upgrades"] = new Dictionary<long, Upgrade>();
            engine.GlobalProperties["DoClick"] = (Action)(() => DoClick(engine));

            
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
            engine.Scripting.EvaluateStringAsScript("player.points.quantity = player.points.quantity + player.points.click_income");
        }

        public void AssertReady()
        {

        }

        public void SetConfiguration(IdleEngine engine)
        {
            engine.State.AddHandler(States.GAMEPLAY, "gainResource", (ie, args) =>
            {
                if(args.Length < 2)
                {
                    throw new InvalidOperationException("Need resource id in position 1.");
                }
                if(args.Length < 3)
                {
                    throw new InvalidOperationException("Need resource quantity in position 2.");
                }
                engine.Scripting.EvaluateStringAsScript("player.Points.Quantity = player.Points.Quantity + quantity", Tuple.Create<string, object>("quantity", BigDouble.Parse(args[2])));
            }, "gainResource [resourceId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "click", (ie, args) =>
            {
                engine.Scripting.EvaluateStringAsScript("player.Points.Quantity = player.Points.Quantity + player.Points.click_income").ToObject<BigDouble>();
            }, "click [times?]");

            engine.State.AddHandler(States.GAMEPLAY, "spendResource", (ie, args) =>
            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need resource id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need resource quantity in position 2.");
                }
                var neededQuantity = BigDouble.Parse(args[2]);
                if(ie.GetPlayer().Points.Quantity < neededQuantity)
                {
                    throw new InvalidOperationException(String.Format("Need {0} {1} but had {2}.", neededQuantity, args[1], ie.GetPlayer().Points.Quantity));
                }
                engine.Scripting.EvaluateStringAsScript("player.Points.Quantity = player.Points.Quantity - quantity", Tuple.Create<string, object>("quantity", neededQuantity));
            }, "spendResource [resourceId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "loseResource", (ie, args) =>
            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need resource id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need resource quantity in position 2.");
                }
                var neededQuantity = BigDouble.Parse(args[2]);
                engine.Scripting.EvaluateStringAsScript("player.Points.Quantity = player.Points.Quantity - quantity", Tuple.Create<string, object>("quantity", neededQuantity));
            }, "loseResource [resourceId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "gainProducer", (ie, args) =>
            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need producer id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need producer quantity in position 2.");
                }
                engine.GetPlayer().Producers[long.Parse(args[1])].Quantity += BigDouble.Parse(args[2]);
            }, "gainProducer [producerId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "buyProducer", (ie, args) =>

            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need producer id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need producer quantity in position 2.");
                }
                var producer = engine.GetProducers()[long.Parse(args[1])];
                var neededQuantity = BigDouble.Parse(args[2]);
                if(!engine.GetPlayer().CanAfford(producer, neededQuantity))
                {
                    throw new InvalidOperationException(String.Format("Need {0} points but had {1}.", engine.GetPlayer().CalculateCost(producer, neededQuantity), engine.GetPlayer().Points.Quantity));
                }
                ie.GetPlayer().BuyProducer(producer.Id);
            }, "buyProducer [producerId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "loseProducer", (ie, args) =>
            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need producer id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need producer quantity in position 2.");
                }
                engine.GetPlayer().Producers[long.Parse(args[1])].Quantity -= BigDouble.Parse(args[2]);
            }, "loseProducer [producerId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "gainUpgrade", (ie, args) =>
            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need upgrade id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need upgrade quantity in position 2.");
                }
                engine.GetPlayer().Upgrades[long.Parse(args[1])].Quantity += BigDouble.Parse(args[2]);
            }, "gainUpgrade [upgradeId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "buyUpgrade", (ie, args) =>

            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need upgrade id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need upgrade quantity in position 2.");
                }
                var upgrade = engine.GetUpgrades()[long.Parse(args[1])];
                var neededQuantity = BigDouble.Parse(args[2]);
                if (!engine.GetPlayer().CanAfford(upgrade, neededQuantity))
                {
                    throw new InvalidOperationException(String.Format("Need {0} points but had {1}.", engine.GetPlayer().CalculateCost(upgrade, neededQuantity), engine.GetPlayer().Points.Quantity));
                }
                ie.GetPlayer().BuyUpgrade(upgrade.Id);
            }, "buyUpgrade [upgradeId] [quantity]");

            engine.State.AddHandler(States.GAMEPLAY, "loseUpgrade", (ie, args) =>
            {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("Need upgrade id in position 1.");
                }
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("Need upgrade quantity in position 2.");
                }
                engine.GetPlayer().Upgrades[long.Parse(args[1])].Quantity = BigDouble.Max(BigDouble.Zero, engine.GetPlayer().Upgrades[long.Parse(args[1])].Quantity - BigDouble.Parse(args[2]));
            }, "loseUpgrade [upgradeId] [quantity]");

            engine.State.DefineTransition(StateMachine.DEFAULT_STATE, States.GAMEPLAY);
        }

        public void SetDefinitions(IdleEngine engine)
        {
            
        }

        public string[] GetScriptLocations()
        {
            return new string[0];
        }

        public void LoadScripts(IdleEngine engine)
        {
            throw new NotImplementedException();
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

        public static BigDouble CalculatePurchaseCost(this IdleEngine engine, IBuyable buyable, BigDouble alreadyOwnedQuantity, BigDouble toBuyQuantity)
        {
            BigDouble total = 0;
            BigDouble nextQuantity = alreadyOwnedQuantity;
            while(nextQuantity < toBuyQuantity + alreadyOwnedQuantity)
            {
                total += engine.Scripting.EvaluateStringAsScript(buyable.CostExpression).ToObject<BigDouble>() * new BigDouble(1.15).Pow(nextQuantity);
                nextQuantity++;
            }
            return total;
        }
    }


}