using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ClickerPlayer : Entity
    {
        public ClickerPlayer(IdleEngine engine) : base(engine)
        {
            this.Points = new PointsHolder();
            this.Producers = new Dictionary<long, Producer>();
            this.Upgrades = new List<long>();
            foreach(var producer in engine.GetProducers())
            {
                Producers[producer.Key] = producer.Value;
            }
        }

        public PointsHolder Points { get; }
        public Dictionary<long, Producer> Producers { get; }
        public List<long> Upgrades { get; }

        protected override void CustomUpdate(IdleEngine engine, float deltaTime)
        {
            Points.Quantity += Points.TotalIncome * deltaTime;
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
            }
        }

        public void BuyUpgrade(long id)
        {
            Upgrade upgrade = Engine.GetUpgrades()[id];
            BigDouble cost = Engine.Scripting.Evaluate(upgrade.CostExpression, new Dictionary<string, object>()
            {
                { "target", upgrade }
            }).ToObject<BigDouble>();
            if (!Upgrades.Contains(id) &&SpendPoints(cost))
            {
                AddModifier(upgrade);
                this.Upgrades.Add(upgrade.Id);
            }
        }
    }
}