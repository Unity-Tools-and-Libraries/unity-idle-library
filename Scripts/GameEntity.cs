using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    public class GameEntity : EntityDefinitionProperties
    {
        private readonly IdleEngine engine;
        private BigDouble _quantity = 0;
        private BigDouble _progress = 0;
        public readonly EntityDefinition definition;
        private Dictionary<string, BigDouble> requirements = new Dictionary<string, BigDouble>();
        private Dictionary<string, BigDouble> costs = new Dictionary<string, BigDouble>();
        private Dictionary<string, BigDouble> productionInputs = new Dictionary<string, BigDouble>();
        private Dictionary<string, BigDouble> productionOutputs = new Dictionary<string, BigDouble>();
        private Dictionary<string, BigDouble> upkeep = new Dictionary<string, BigDouble>();
        private Dictionary<string, BigDouble> minimumProduction = new Dictionary<string, BigDouble>();
        private StateMatcher hideEntityMatcher;
        private StateMatcher disableEntityMatcher;

        public string EntityKey => definition.EntityKey;
        public string Name => definition.Name;
        public BigDouble StartingQuantity => definition.StartingQuantity;

        public bool IsEnabled => !ShouldBeDisabled(engine);

        public Dictionary<string, PropertyReference> BaseRequirements => definition.BaseRequirements;
        public Dictionary<string, PropertyReference> BaseCosts => definition.BaseCosts;
        public Dictionary<string, PropertyReference> BaseProductionInputs => definition.BaseProductionInputs;
        public Dictionary<string, PropertyReference> BaseProductionOutputs => definition.BaseProductionOutputs;
        public Dictionary<string, PropertyReference> BaseUpkeep => definition.BaseUpkeep;
        public BigDouble Quantity {
            get {
                var actualQuantity = _quantity;
                var cap = QuantityCap != null ? QuantityCap.Get(engine) : _quantity;
                if (actualQuantity > cap)
                {
                    return cap;
                }
                return actualQuantity;
           }
        }
        public BigDouble Progress => _progress;
        public ISet<string> Types => definition.Types;
        public bool ScaleProductionOnAvailableInputs => definition.ScaleProductionOnAvailableInputs;
        public StateMatcher HiddenMatcher => hideEntityMatcher;
        public StateMatcher DisabledMatcher => disableEntityMatcher;
        public PropertyReference QuantityCap => definition.QuantityCap;

        /*
         * The quantities of entities which are required when trying to buy this entity.
         */
        public Dictionary<string, BigDouble> Requirements => requirements;
        /*
         * The entities and quantities which are consumed to buy this entity.
         */
        public Dictionary<string, BigDouble> Costs => costs;
        /*
         * The entities and quantities which are consumed each tick by this entity and if a shortfall of these requirements causes the loss of this entity.
         */
        public Dictionary<string, BigDouble> Upkeep => upkeep;
        /*
         * The entities and quantities which are consumed by this entity as inputs to their production.
         */
        public Dictionary<string, BigDouble> ProductionInputs => productionInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, BigDouble> ProductionOutputs => productionOutputs;
        public Dictionary<string, BigDouble> MinimumProductionOutputs => minimumProduction;
        public ISet<ModifierDefinition> Modifiers => ((EntityDefinitionProperties)definition).Modifiers;
        public Dictionary<string, PropertyReference> BaseMinimumProductionOutputs => definition.BaseMinimumProductionOutputs;
        public bool CanBeBought => definition.CanBeBought;
        public BigDouble RealQuantity => _quantity;

        public GameEntity(EntityDefinition definition, IdleEngine engine)
        {
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            this.engine = engine;
        }

        public void Buy(BigDouble quantityToBuy, bool buyAllOrNone)
        {
            engine.BuyEntity(this, quantityToBuy, buyAllOrNone);
        }

        public void Buy(BigDouble quantityToBuy)
        {
            Buy(quantityToBuy, false);
        }

        public bool RequirementAreMet()
        {
            var requirementsMet = true;
            foreach(var requirement in Requirements)
            {
                requirementsMet = engine.AllEntities[requirement.Key].Quantity >= requirement.Value;
            }
            return requirementsMet;
        }

        internal void AddModifier(ModifierDefinition modifier)
        {
            throw new NotImplementedException();
        }

        /**
         * Determine the number of entities which are able to produce.
         */
        public BigDouble DetermineProduction()
        {
            var quantityAbleToProduce = Quantity;
            foreach (var requirement in ProductionInputs)
            {
                var quantityWithSufficientInputs = BigDouble.Min(engine.AllEntities[requirement.Key].Quantity / requirement.Value, this.Quantity);
                if(!ScaleProductionOnAvailableInputs)
                {
                    quantityAbleToProduce = 0;
                    break;
                }
            }
            return quantityAbleToProduce;
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
            if (_progress >= 1)
            {
                _progress = 0;
                _quantity += 1;
            }
        }

        public void SetProgress(int newProgress)
        {
            _progress = newProgress % 1000;
        }

        public bool ShouldBeHidden(IdleEngine engine)
        {
            return definition.HiddenMatcher.Matches(engine);
        }

        public bool ShouldBeDisabled(IdleEngine engine)
        {
            return definition.DisabledMatcher.Matches(engine);
        }

        public override string ToString()
        {
            return String.Format("GameEntity({0}) x {1}", this.Name, this.Quantity);
        }

        public enum ModifiableProperty
        {
            UPKEEP,
            PRODUCTION_INPUTS,
            PRODUCTION_OUTPUTS,
            COST,
            REQUIREMENTS
        }
    }
}