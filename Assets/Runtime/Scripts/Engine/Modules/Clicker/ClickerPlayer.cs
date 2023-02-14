using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using System.Linq;
using System.Collections.Generic;
using System;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerPlayer : Player
    {
        public ClickerPlayer(IdleEngine engine, long id, Dictionary<string, BigDouble> resources = null) : base(engine, id, resources != null ? resources :
            new Dictionary<string, BigDouble>() { { "points", BigDouble.Zero } })
        {
            this.Producers = new Dictionary<long, Producer>();
            this.Upgrades = new Dictionary<long, Upgrade>();
            foreach (var upgrade in engine.GetUpgrades())
            {
                Upgrades[upgrade.Key] = upgrade.Value;
            }
            foreach (var producer in engine.GetProducers())
            {
                Producers[producer.Key] = producer.Value;
            }
        }

        public Dictionary<long, Producer> Producers { get; }
        public Dictionary<long, Upgrade> Upgrades { get; }

        protected override void CustomUpdate(IdleEngine engine, float deltaTime)
        {
            base.CustomUpdate(engine, deltaTime);
            foreach (var p in Producers.Values)
            {
                p.IsUnlocked = engine.Scripting.EvaluateStringAsScript(p.UnlockExpression, new Dictionary<string, object>()
                {
                    { "producer", p },
                    { "target", this }
                }).Boolean;
                p.IsEnabled = engine.Scripting.EvaluateStringAsScript(p.EnableExpression, new Dictionary<string, object>()
                {
                    { "producer", p },
                    { "target", this }
                }).Boolean;
            }
            foreach (var u in Upgrades.Values)
            {
                bool wasUnlocked = u.IsUnlocked;
                bool wasEnabled = u.IsEnabled;
                u.IsUnlocked = engine.Scripting.EvaluateStringAsScript(u.UnlockExpression, new Dictionary<string, object>()
                {
                    { "upgrade", u },
                    { "target", this }
                }).Boolean;
                u.IsEnabled = engine.Scripting.EvaluateStringAsScript(u.EnableExpression, new Dictionary<string, object>()
                {
                    { "upgrade", u },
                    { "target", this }
                }).Boolean;
                if(!wasUnlocked && u.IsUnlocked)
                {
                    engine.Emit("UpgradeUnlocked", Tuple.Create<string, object>("upgrade", u));
                }
                if(!wasEnabled && u.IsEnabled)
                {
                    engine.Emit("UpgradeEnabled", Tuple.Create<string, object>("upgrade", u));
                }
            }
        }

        public Dictionary<string, BigDouble> CalculateCost(IBuyable buyable, BigDouble quantity)
        {
            BigDouble startingQuantity = 0;
            if (buyable is Producer)
            {
                startingQuantity = Producers[(buyable as Producer).Id].Quantity;
            } else if (buyable is Upgrade)
            {
                startingQuantity = Upgrades[(buyable as Upgrade).Id].Quantity;
            }
            return Engine.CalculatePurchaseCost(buyable, startingQuantity, quantity);
        }

        public bool CanAfford(IBuyable buyable, BigDouble quantity)
        {
            return CanSpend(CalculateCost(buyable, quantity));
        }

        public bool CanSpend(Dictionary<string, BigDouble> resources)
        {
            return resources.All(resource => GetResource(resource.Key).Quantity >= resource.Value);
        }

        public void BuyProducer(long id)
        {
            Producer producerDefinition = Producers[id];
            Dictionary<string, BigDouble> cost = CalculateCost(producerDefinition, 1);
            if (SpendIfAble(cost))
            {
                producerDefinition.Quantity += 1;
                var boughtProducerEvent = new ProducerBoughtEvent(producerDefinition);
                Emit(ProducerBoughtEvent.EventName, boughtProducerEvent);
                Engine.Emit(ProducerBoughtEvent.EventName, boughtProducerEvent);
                RecalculateIncome();
            }
        }
        /**
         * Attempts to spend the given resources, returning true if able.
         * 
         * Return false if it failed to buy and keep resources the same.
         */
        private bool SpendIfAble(Dictionary<string, BigDouble> cost)
        {
            bool buying = CanSpend(cost);
            if(buying)
            {
                foreach(var resource in cost)
                {
                    GetResource(resource.Key).Spend(resource.Value);
                }
            }
            return buying;
        }

        public void BuyUpgrade(long id)
        {
            Upgrade upgrade = Engine.GetPlayer<ClickerPlayer>().Upgrades[id];
            Dictionary<string, BigDouble> cost = Engine.GetPlayer<ClickerPlayer>().CalculateCost(upgrade, 1);
            if (SpendIfAble(cost) && (!GetModifiers().Contains(upgrade.Id) || upgrade.MaxQuantity > upgrade.Quantity))
            {
                AddModifier(upgrade);
                var upgradeBoughtEvent = new UpgradeBoughtEvent(upgrade);
                Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
                RecalculateIncome();
                upgrade.Quantity++;
                if(upgrade.Quantity == upgrade.MaxQuantity)
                {
                    var maxLevelEvent = new MaxLevelReachedEvent(upgrade, upgrade.Quantity);
                    upgrade.Emit(MaxLevelReachedEvent.EventName, maxLevelEvent);
                    Emit(MaxLevelReachedEvent.EventName, maxLevelEvent);
                }
            }
        }

        private void RecalculateIncome()
        {
            GetResource("points").TotalIncome = Producers.Values.Aggregate(BigDouble.Zero, (total, p) => total + p.TotalOutput);
        }
    }
}