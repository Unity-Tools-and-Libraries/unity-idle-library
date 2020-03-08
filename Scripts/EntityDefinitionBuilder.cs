using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    public class EntityDefinitionBuilder : EntityDefinitionProperties
    {
        private ISet<string> types = new HashSet<string>();
        private string key;
        private string name;
        private BigDouble innateIncome;
        private Dictionary<string, BigDouble> requires = new Dictionary<string, BigDouble>();
        private Dictionary<string, BigDouble> costs = new Dictionary<string, BigDouble>();

        private Dictionary<string, ConsumptionDefinition> consumes = new Dictionary<string, ConsumptionDefinition>();
        private Dictionary<string, BigDouble> production = new Dictionary<string, BigDouble>();
        private BigDouble startingQuantity = 0;

        public EntityDefinitionBuilder(string key)
        {
            this.key = key;
        }

        public BigDouble InnateIncome => innateIncome;

        public string EntityKey => key;

        public string Name => name;

        public ISet<string> Types => types;

        public BigDouble StartingQuantity => startingQuantity;

        public Dictionary<string, BigDouble> Requires => requires;

        public Dictionary<string, BigDouble> Costs => costs;

        public Dictionary<string, ConsumptionDefinition> Consumes => consumes;
        public Dictionary<string, BigDouble> Produces => production;

        /*
         * Set the displayed name of this entity.
         */
        public EntityDefinitionBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public EntityDefinitionBuilder WithRequirement(string entityRequired, BigDouble quantityRequired)
        {
            requires.Add(entityRequired, quantityRequired);
            return this;
        }

        public EntityDefinitionBuilder WithCost(string entityRequired, BigDouble quantityRequired)
        {
            costs.Add(entityRequired, quantityRequired);
            return this;
        }

        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick, bool loseOnShortfall, bool disableOnShortfall)
        {
            consumes.Add(entity, new ConsumptionDefinition(entity, quantityConsumedPerTick, loseOnShortfall, disableOnShortfall));
            return this;
        }

        public EntityDefinitionBuilder WithProduction(string entityKey, BigDouble progressAddedPerTick)
        {
            production.Add(entityKey, progressAddedPerTick);
            return this;
        }

        public EntityDefinitionBuilder WithType(string type)
        {
            types.Add(type);
            return this;
        }

        public EntityDefinitionBuilder WithStartingQuantity(BigDouble startingQuantity)
        {
            this.startingQuantity = startingQuantity;
            return this;
        }
    }

    public interface EntityDefinitionProperties
    {
        ISet<string> Types { get;  }
        string EntityKey { get; }
        string Name { get; }
        BigDouble InnateIncome { get; }
        BigDouble StartingQuantity { get; }
        Dictionary<string, BigDouble> Requires { get; }
        Dictionary<string, BigDouble> Costs { get; }
        Dictionary<string, ConsumptionDefinition> Consumes { get; }
        Dictionary<string, BigDouble> Produces { get; }
    }
}