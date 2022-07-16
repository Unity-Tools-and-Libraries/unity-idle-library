using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerPlayer : Entity
    {
        public ClickerPlayer(IdleEngine engine, long id) : base(engine, id)
        {
            this.Points = new PointsHolder();
            this.Producers = new Dictionary<long, Producer>();
            this.Upgrades = new Dictionary<long, Upgrade>();
            foreach(var upgrade in engine.GetUpgrades())
            {
                Upgrades[upgrade.Key] = upgrade.Value;
            }
            foreach(var producer in engine.GetProducers())
            {
                Producers[producer.Key] = producer.Value;
            }
        }

        public PointsHolder Points { get; }
        public Dictionary<long, Producer> Producers { get; }
        public Dictionary<long, Upgrade> Upgrades { get; }

        protected override void CustomUpdate(IdleEngine engine, float deltaTime)
        {
            Points.Quantity += Points.TotalIncome * deltaTime;
            foreach(var p in Producers.Values)
            {
                p.IsUnlocked = engine.Scripting.Evaluate(p.UnlockExpression, new Dictionary<string, object>()
                {
                    { "producer", p }
                }).Boolean;
                p.IsEnabled = engine.Scripting.Evaluate(p.EnableExpression, new Dictionary<string, object>()
                {
                    { "producer", p }
                }).Boolean;
            }
            foreach (var u in Upgrades.Values)
            {
                u.IsUnlocked = engine.Scripting.Evaluate(u.UnlockExpression, new Dictionary<string, object>()
                {
                    { "upgrade", u }
                }).Boolean;
                u.IsEnabled = engine.Scripting.Evaluate(u.EnableExpression, new Dictionary<string, object>()
                {
                    { "upgrade", u }
                }).Boolean;
            }
        }
        /*
         * Attempt to spend the given amount of points. Returns true if successful.
         */
        public bool SpendPoints(BigDouble amount)
        {
            if(Points.Quantity >= amount)
            {
                Points.Quantity -= amount;
                return true;
            }
            return false;
        }

        public void BuyProducer(long id)
        {
            Producer producerDefinition = Engine.GetProducers()[id];
            BigDouble cost = Engine.Scripting.Evaluate(producerDefinition.CostExpression, new Dictionary<string, object>()
            {
                { "target", producerDefinition }
            }).ToObject<BigDouble>();
            if(SpendPoints(cost))
            {
                producerDefinition.Quantity += 1;
                var boughtProducerEvent = new ProducerBoughtEvent(producerDefinition);
                Emit(ProducerBoughtEvent.EventName, boughtProducerEvent);
                Engine.Emit(ProducerBoughtEvent.EventName, boughtProducerEvent);
            }
        }

        public void BuyUpgrade(long id)
        {
            Upgrade upgrade = Engine.GetUpgrades()[id];
            BigDouble cost = Engine.Scripting.Evaluate(upgrade.CostExpression, new Dictionary<string, object>()
            {
                { "target", upgrade }
            }).ToObject<BigDouble>();
            if (!GetModifiers().Contains(upgrade.Id) && SpendPoints(cost))
            {
                AddModifier(upgrade);
                var upgradeBoughtEvent = new UpgradeBoughtEvent(upgrade);
                Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
                Engine.Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
            }
        }
    }
}