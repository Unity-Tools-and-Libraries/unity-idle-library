using BreakInfinity;
using System.Numerics;

namespace IdleFramework
{
    public class ConsumptionDefinition
    {
        private readonly string entity;
        private readonly BigDouble quantity;
        private readonly bool entityLostOnShortfall;
        private readonly bool productionBlockedOnShortfall;

        public ConsumptionDefinition(string entity, BigDouble quantity, bool entityLostOnShortfall, bool productionBlockedOnShortfall)
        {
            this.entity = entity;
            this.quantity = quantity;
            this.entityLostOnShortfall = entityLostOnShortfall;
            this.productionBlockedOnShortfall = productionBlockedOnShortfall;
        }

        public string Entity => entity;

        public BigDouble Quantity => quantity;

        public bool EntityLostOnShortfall => entityLostOnShortfall;

        public bool ProductionBlockedOnShortfall => productionBlockedOnShortfall;
    }
}