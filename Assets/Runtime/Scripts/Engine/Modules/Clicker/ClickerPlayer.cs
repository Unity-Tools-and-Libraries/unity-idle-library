using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerPlayer : Player
    {
        public ClickerPlayer(IdleEngine engine, long id) : base(engine, id)
        {
            this.Producers = new Dictionary<double, ProducerInstance>();
            this.Upgrades = new Dictionary<double, UpgradeInstance>();
        }

        public Dictionary<double, ProducerInstance> Producers { get; private set; }
        public Dictionary<double, UpgradeInstance> Upgrades { get; private set; }

        public override Entity Initialize()
        {
            foreach (var upgrade in Engine.GetUpgrades())
            {
                if(!Upgrades.ContainsKey(upgrade.Key))
                {
                    Upgrades[upgrade.Key] = new UpgradeInstance(Engine, upgrade.Key);
                }
            }
            foreach (var producer in Engine.GetProducers())
            {
                if (!Producers.ContainsKey(producer.Key))
                {
                    Producers[producer.Key] = new ProducerInstance(Engine, producer.Key);
                }
            }
            return this;
        }

        protected override void CustomUpdate(IdleEngine engine, float deltaTime)
        {
            base.CustomUpdate(engine, deltaTime);
            foreach (var p in Producers.Values)
            {
                var wasUnlocked = p.IsUnlocked;
                var wasEnabled = p.IsEnabled;
                p.IsUnlocked = p.IsUnlocked || engine.Scripting.EvaluateStringAsScript(
                    engine.GetProducers()[p.ProducerId].UnlockExpression, new Dictionary<string, object>()
                {
                    { "producer", p },
                    { "target", this }
                }).Boolean;
                p.IsEnabled = engine.Scripting.EvaluateStringAsScript(
                    engine.GetProducers()[p.ProducerId].EnableExpression, new Dictionary<string, object>()
                {
                    { "producer", p },
                    { "target", this }
                }).Boolean;
                if(!wasUnlocked && p.IsUnlocked)
                {
                    engine.Logging.Log(string.Format("Emitting unlock event for producer {0}", p.ProducerId), "events");
                    engine.GetProducers()[p.ProducerId].Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("producer", p));
                    p.Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("producer", p));
                    Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("producer", p));
                }
                if(!wasEnabled && p.IsEnabled)
                {
                    engine.Logging.Log(string.Format("Emitting enable event for producer {0}", p.ProducerId), "events");
                    engine.GetProducers()[p.ProducerId].Emit(IsEnabledChangedEvent.EventName, Tuple.Create<string, object>("producer", p));
                    p.Emit(IsEnabledChangedEvent.EventName, Tuple.Create<string, object>("producer", p));
                    Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("producer", p));
                }
            }
            foreach (var u in Upgrades.Values)
            {
                bool wasUnlocked = u.IsUnlocked;
                bool wasEnabled = u.IsEnabled;
                u.IsUnlocked = engine.Scripting.EvaluateStringAsScript(
                    engine.GetUpgrades()[u.UpgradeId].UnlockExpression, new Dictionary<string, object>()
                {
                    { "upgrade", u },
                    { "target", this }
                }).Boolean;
                u.IsEnabled = engine.Scripting.EvaluateStringAsScript(
                    engine.GetUpgrades()[u.UpgradeId].EnableExpression, new Dictionary<string, object>()
                {
                    { "upgrade", u },
                    { "target", this }
                }).Boolean;
                if(!wasUnlocked && u.IsUnlocked)
                {
                    engine.Logging.Log(string.Format("Emitting unlock event for upgrade {0}", u.UpgradeId), "events");
                    engine.GetUpgrades()[u.UpgradeId].Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("upgrade", u));
                    u.Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("upgrade", u));
                    Emit(IsUnlockedChangeEvent.EventName, Tuple.Create<string, object>("upgrade", u));
                }
                if(!wasEnabled && u.IsEnabled)
                {
                    engine.Logging.Log(string.Format("Emitting enable event for upgrade {0}", u.UpgradeId), "events");
                    engine.GetUpgrades()[u.UpgradeId].Emit(IsEnabledChangedEvent.EventName, Tuple.Create<string, object>("upgrade", u));
                    u.Emit(IsEnabledChangedEvent.EventName, Tuple.Create<string, object>("upgrade", u));
                    Emit(IsEnabledChangedEvent.EventName, Tuple.Create<string, object>("upgrade", u));
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
            Producer producerDefinition = this.Engine.GetProducers()[id];
            Dictionary<string, BigDouble> cost = CalculateCost(producerDefinition, 1);
            if (SpendIfAble(cost))
            {
                Producers[id].Quantity += 1;
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

        public bool CanAddUpgrade(double id)
        {
            Upgrade upgradeDefinition = Engine.GetUpgrades()[id];
            Dictionary<string, BigDouble> cost = Engine.GetPlayer<ClickerPlayer>().CalculateCost(upgradeDefinition, 1);
            return (!GetModifiers().Contains(upgradeDefinition.Id) || upgradeDefinition.MaxQuantity > Upgrades[id].Quantity);
        }

        public void AddUpgrade(double id)
        {
            Upgrade upgradeDefinition = Engine.GetUpgrades()[id];
            Dictionary<string, BigDouble> cost = Engine.GetPlayer<ClickerPlayer>().CalculateCost(upgradeDefinition, 1);
            if (CanAddUpgrade(id))
            {
                AddModifier(upgradeDefinition);
                Upgrades[id].Quantity++;
                var upgradeBoughtEvent = new UpgradeBoughtEvent(upgradeDefinition);
                upgradeDefinition.Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
                Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
                RecalculateIncome();
                if (Upgrades[id].Quantity == upgradeDefinition.MaxQuantity)
                {
                    var maxLevelEvent = new MaxLevelReachedEvent(upgradeDefinition, Upgrades[id].Quantity);
                    Upgrades[id].Emit(MaxLevelReachedEvent.EventName, maxLevelEvent);
                    upgradeDefinition.Emit(MaxLevelReachedEvent.EventName, maxLevelEvent);
                    Emit(MaxLevelReachedEvent.EventName, maxLevelEvent);
                }
            }
        }

        public void BuyUpgrade(double id)
        {
            Upgrade upgradeDefinition = Engine.GetUpgrades()[id];
            Dictionary<string, BigDouble> cost = Engine.GetPlayer<ClickerPlayer>().CalculateCost(upgradeDefinition, 1);
            if (CanAddUpgrade(id) && SpendIfAble(cost))
            {
                AddUpgrade(id);
            }
        }

        private void RecalculateIncome()
        {
            GetResource("points").TotalIncome = Producers.Values.Aggregate(BigDouble.Zero, (total, p) => total + p.TotalOutput);
        }
    }
}