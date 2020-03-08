using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    public class GameEntity: EntityDefinitionProperties
    {
        private readonly IdleEngine engine;
        private BigDouble _quantity = 0;
        private BigDouble _progress = 0;
        public readonly EntityDefinition definition;

        public bool IsResource => definition.IsResource;

        public string EntityKey => definition.EntityKey;

        public string Name => definition.Name;

        public BigDouble InnateIncome => definition.InnateIncome;

        public BigDouble StartingQuantity => definition.StartingQuantity;

        public Dictionary<string, BigDouble> Requires => definition.Requires;

        public Dictionary<string, BigDouble> Costs => definition.Costs;

        public Dictionary<string, ConsumptionDefinition> Consumes => definition.Consumes;

        public Dictionary<string, BigDouble> Produces => definition.Produces;

        public BigDouble Quantity => _quantity;

        public BigDouble Progress => _progress;

        public ISet<string> Types => definition.Types;

        public GameEntity(EntityDefinition definition, IdleEngine engine)
        {
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            this.engine = engine;
        }

        public void Update()
        {
            // Determine amount of production current resources allow.
            Dictionary<string, BigDouble> productionResults = DetermineProduction();

            engine.UpdateResourcesFromEntityProduction(productionResults);

            // Determine quantity of entity supported by resources
            BigDouble quantityConsuming = Quantity;
            foreach (var resource in Consumes)
            {
                quantityConsuming = BigDouble.Min(Quantity, engine.Resources[resource.Key].Quantity / resource.Value.Quantity);
                if (resource.Value.EntityLostOnShortfall) {
                    quantityConsuming = BigDouble.Min(Quantity, quantityConsuming);
                }
            }

            engine.UpdateResourcesFromEntityConsumption(this, quantityConsuming);
            
            _quantity = quantityConsuming;
        }

        internal void AddModifier(Modifier modifier)
        {
            throw new NotImplementedException();
        }

        /**
         * Determine the amount of
         * 
         * Returns the entities and quantities produced.
         */
        public Dictionary<string, BigDouble> DetermineProduction()
        {
            var quantityAbleToProduce = Quantity;
            foreach (var requirement in Consumes)
            {
                if(requirement.Value.ProductionBlockedOnShortfall)
                {
                    quantityAbleToProduce = BigDouble.Min(engine.Resources[requirement.Key].Quantity / requirement.Value.Quantity, this.Quantity);
                }
            }
            var produced = new Dictionary<string, BigDouble>();
            foreach(var production in Produces)
            {
                produced.Add(production.Key, production.Value * quantityAbleToProduce);
            }
            return produced;
        }

        public void ChangeQuantity(BigDouble changeBy)
        {
            _quantity += changeBy;
        }

        public void SetQuantity(BigDouble newQuantity)
        {
            _quantity = newQuantity;
        }

        public void ChangeProgress(BigDouble changeBy)
        {
            _progress += changeBy;
            var overflow = _progress.Floor();
            _quantity += overflow;
            _progress -= overflow;
        }

        public void SetProgress(int newProgress)
        {
            _progress = newProgress % 1000;
        }
    }
}