using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using System.Linq;
using System.Collections.Generic;
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
            }
        }

        public BigDouble CalculateCost(IBuyable buyable, BigDouble quantity)
        {
            BigDouble startingQuantity = 0;
            if (buyable is Producer)
            {
                startingQuantity = Producers[(buyable as Producer).Id].Quantity;
            }
            return Engine.CalculatePurchaseCost(buyable, startingQuantity, quantity);
        }

        public bool CanAfford(IBuyable buyable, BigDouble quantity)
        {
            return GetResource("points").Quantity >= CalculateCost(buyable, quantity);
        }

        public void BuyProducer(long id)
        {
            Producer producerDefinition = Producers[id];
            BigDouble cost = CalculateCost(producerDefinition, 1);
            if (GetResource("points").Spend(cost))
            {
                producerDefinition.Quantity += 1;
                var boughtProducerEvent = new ProducerBoughtEvent(producerDefinition);
                Emit(ProducerBoughtEvent.EventName, boughtProducerEvent);
                Engine.Emit(ProducerBoughtEvent.EventName, boughtProducerEvent);
                RecalculateIncome();
            }
        }

        public void BuyUpgrade(long id)
        {
            Upgrade upgrade = Engine.GetPlayer().Upgrades[id];
            BigDouble cost = Engine.GetPlayer().CalculateCost(upgrade, 1);
            if (!GetModifiers().Contains(upgrade.Id) && GetResource("points").Spend(cost))
            {
                AddModifier(upgrade);
                var upgradeBoughtEvent = new UpgradeBoughtEvent(upgrade);
                Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
                Engine.Emit(UpgradeBoughtEvent.EventName, upgradeBoughtEvent);
                RecalculateIncome();
            }
        }

        private void RecalculateIncome()
        {
            GetResource("points").TotalIncome = Producers.Values.Aggregate(BigDouble.Zero, (total, p) => total + p.TotalOutput);
        }
    }
}