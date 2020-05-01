using BreakInfinity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace IdleFramework.Entities
{
    public class EntitySnapshot : Snapshot
    {
        private readonly string entityKey;
        private readonly BigDouble quantity;
        private readonly BigDouble quantityBought;
        private readonly BigDouble highestQuantityAchieved;

        public EntitySnapshot(string entityKey, BigDouble quantity, BigDouble quantityBought, BigDouble highestQuantityAchieved)
        {
            this.entityKey = entityKey;
            this.quantity = quantity;
            this.quantityBought = quantityBought;
            this.highestQuantityAchieved = highestQuantityAchieved;
        }
        public string EntityKey => entityKey;
        public BigDouble Quantity => quantity;
        public BigDouble QuantityBought => quantityBought;
        public BigDouble HighestQuantityAchieved => highestQuantityAchieved;
    }
}