using System.Collections.Generic;
using System;
using System.Numerics;
using BreakInfinity;

namespace IdleFramework
{
    public class EntityDefinition : EntityDefinitionProperties
    {
        private static HashSet<string> usedEntityKeys = new HashSet<string>();
        private readonly string entityKey;
        private string name;
        private bool isResource;
        private BigDouble innateIncome;
        private BigDouble startingQuantity;
        private Dictionary<string, ConsumptionDefinition> consumption;
        private Dictionary<string, BigDouble> production;
        private ISet<string> types;
        private Dictionary<string, BigDouble> requirements;
        private Dictionary<string, BigDouble> costs;

        public string EntityKey => entityKey;
        public string Name => name;
        public bool IsResource => isResource;
        public BigDouble InnateIncome => innateIncome;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, ConsumptionDefinition> Consumes => consumption;
        public Dictionary<string, BigDouble> Produces => production;
        public Dictionary<string, BigDouble> Requires => requirements;
        public Dictionary<string, BigDouble> Costs => costs;
        public ISet<string> Types => types;

        internal EntityDefinition(EntityDefinitionProperties definitionProperties)
        {
            if (!usedEntityKeys.Add(definitionProperties.EntityKey))
            {
                throw new InvalidOperationException(String.Format("Attempted to use the key {0} multiple times, which is not supported.", definitionProperties.EntityKey));
            };
            this.entityKey = definitionProperties.EntityKey;
            innateIncome = definitionProperties.InnateIncome;
            name = definitionProperties.Name ;
            startingQuantity = definitionProperties.StartingQuantity;
            this.consumption = definitionProperties.Consumes;
            this.production = definitionProperties.Produces;
            this.types = definitionProperties.Types;
        }

        
    }
}