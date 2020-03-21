using BreakInfinity;
using IdleFramework;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    public class GameEntity : Modifier, Updates
    {
        private readonly IdleEngine engine;
        private readonly ISet<Updates> updateables = new HashSet<Updates>();
        private ModifiableProperty quantityCap;
        private BigDouble _quantity = 0;
        private BigDouble _progress = 0;
        private readonly EntityDefinition definition;
        private readonly ModifiableProperty quantityChangePerSecond;
        
        private readonly Dictionary<string, ModifiableProperty> requirements = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> costs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> productionInputs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> productionOutputs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> upkeep = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> minimumProduction = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> customProperties = new Dictionary<string, ModifiableProperty>();

        public string EntityKey => definition.EntityKey;
        public string Name => definition.Name;
        public BigDouble StartingQuantity => definition.StartingQuantity;

        public bool IsEnabled => !ShouldBeDisabled(engine);

        public Dictionary<string, ValueContainer> BaseRequirements => definition.BaseRequirements;
        public Dictionary<string, ValueContainer> BaseCosts => definition.BaseCosts;
        public Dictionary<string, ValueContainer> BaseProductionInputs => definition.BaseProductionInputs;
        public Dictionary<string, ValueContainer> BaseProductionOutputs => definition.BaseProductionOutputs;
        public Dictionary<string, ValueContainer> BaseUpkeep => definition.BaseUpkeep;
        public BigDouble Quantity {
            get {
                var actualQuantity = _quantity;
                var cap = QuantityCap != null ? QuantityCap.GetAsNumber(engine) : _quantity;
                return BigDouble.Min(actualQuantity, cap);
           }
        }
        public BigDouble Progress => _progress;
        public ISet<string> Types => definition.Types;
        public bool ScaleProductionOnAvailableInputs => definition.ScaleProductionOnAvailableInputs;
        public StateMatcher HiddenMatcher => definition.HiddenMatcher;
        public StateMatcher DisabledMatcher => definition.DisabledMatcher;
        public ValueContainer QuantityCap => definition.QuantityCap;

        public bool HasCustomProperty(string propertyName)
        {
            return customProperties.ContainsKey(propertyName);
        }

        /*
         * The quantities of entities which are required when trying to buy this entity.
         */
        public Dictionary<string, ModifiableProperty> Requirements => requirements;
        /*
         * The entities and quantities which are consumed to buy this entity.
         */
        public Dictionary<string, ModifiableProperty> Costs => costs;
        /*
         * The entities and quantities which are consumed each tick by this entity and if a shortfall of these requirements causes the loss of this entity.
         */
        public Dictionary<string, ModifiableProperty> Upkeep => upkeep;
        /*
         * The entities and quantities which are consumed by this entity as inputs to their production.
         */
        public Dictionary<string, ModifiableProperty> ProductionInputs => productionInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, ModifiableProperty> ProductionOutputs => productionOutputs;
        public Dictionary<string, ModifiableProperty> MinimumProductionOutputs => minimumProduction;
        public ISet<ModifierDefinition> Modifiers => definition.Modifiers;
        public Dictionary<string, ValueContainer> BaseMinimumProductionOutputs => definition.BaseMinimumProductionOutputs;
        public bool CanBeBought => definition.CanBeBought;
        public BigDouble RealQuantity => _quantity;
        public Dictionary<string, ModifiableProperty> CustomProperties => customProperties;

        public ModifiableProperty QuantityChangePerSecond => quantityChangePerSecond;

        public GameEntity(EntityDefinition definition, IdleEngine engine): base(definition, new HashSet<Effect>())
        {
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            foreach(var property in definition.CustomProperties)
            {
                customProperties.Add(property.Key, new ModifiableProperty(this, property.Key, property.Value, engine));
            }
            this.engine = engine;
            quantityChangePerSecond = new ModifiableProperty(this, "production-per-second", Literal.Of(0), engine);

            foreach(var updateable in requirements.Values)
            {
                updateables.Add(updateable);
            }
            foreach (var updateable in upkeep.Values)
            {
                updateables.Add(updateable);
            }
            foreach (var updateable in costs.Values)
            {
                updateables.Add(updateable);
            }
            foreach (var updateable in productionInputs.Values)
            {
                updateables.Add(updateable);
            }
            foreach (var updateable in productionOutputs.Values)
            {
                updateables.Add(updateable);
            }
            updateables.Add(QuantityChangePerSecond);
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
                requirementsMet = engine.AllEntities[requirement.Key].Quantity >= requirement.Value.GetAsNumber(engine);
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
                var quantityWithSufficientInputs = BigDouble.Min(engine.AllEntities[requirement.Key].Quantity / requirement.Value.GetAsNumber(engine), this.Quantity);
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
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.GetAsNumber(engine));
        }

        public void SetQuantity(BigDouble newQuantity)
        {
            _quantity = newQuantity;
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.GetAsNumber(engine));
        }

        public void ChangeProgress(BigDouble changeBy)
        {
            _progress += changeBy;
            if (_progress >= 1)
            {
                _progress = 0;
                _quantity += 1;
            }
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.GetAsNumber(engine));
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
            return String.Format("GameEntity({0}) x {1} + {2}/sec", this.EntityKey, this.Quantity, quantityChangePerSecond.GetAsNumber(engine));
        }


        internal ValueContainer GetRawProperty(string entityProperty, string entitySubProperty)
        {
            switch (entityProperty)
            {
                case "outputs":
                    return ProductionOutputs[entitySubProperty];
                case "inputs":
                    return ProductionInputs[entitySubProperty];
                case "quantity":
                    return _quantity.AsContainer();
                case "enabled":
                    return IsEnabled.AsContainer();
                default:
                    assertCustomPropertyExists(entityProperty);
                    return CustomProperties[entityProperty];
            }
        }


        public bool GetPropertyAsBoolean(string entityProperty, string entitySubProperty)
        {
            return GetRawProperty(entityProperty, entitySubProperty).GetAsBoolean(engine);
        }

        public BigDouble GetPropertyAsNumber(string entityProperty, string entitySubProperty)
        {
            return GetRawProperty(entityProperty, entitySubProperty).GetAsNumber(engine);
        }

        public string GetPropertyAsString(string entityProperty, string entitySubProperty)
        {
            return GetRawProperty(entityProperty, entitySubProperty).GetAsString(engine);
        }

        public ModifierEffect AsModifierEffectFor(IdleEngine engine, string subject, string property)
        {
            string modifierKey = String.Format("{0}-{1}-{2}", EntityKey, subject, "production");
            switch(property)
            {
                case "production":
                    return new ModifierEffect(
                        this,
                        new Effect(new EntityPropertyModifierEffectDefinition(subject, "production", 
                        new EntityPropertyReference(this.EntityKey, "outputs", subject)
                        .Minus(new EntityPropertyReference(this.EntityKey, "inputs", subject)), EffectType.ADD), engine)
                        );
                default:
                    throw new InvalidOperationException();
            }
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            foreach(var updateable in updateables)
            {
                updateable.Update(engine, deltaTime);
            }
        }

        private void assertCustomPropertyExists(string property)
        {
            if(!customProperties.ContainsKey(property))
            {
                throw new InvalidOperationException(String.Format("Custom property {0} not defined.", property));
            }
        }
    }

    public static class GameEntityExtensions
    {
        public static void ForEachProperty(this GameEntity entity, Action<string, ModifiableProperty> action)
        {
            foreach(var output in entity.ProductionOutputs)
            {
                action(output.Key, output.Value);
            }
            foreach (var input in entity.ProductionInputs)
            {
                action(input.Key, input.Value);
            }
            foreach (var upkeep in entity.Upkeep)
            {
                action(upkeep.Key, upkeep.Value);
            }
            foreach (var cost in entity.Costs)
            {
                action(cost.Key, cost.Value);
            }
            foreach (var requirement in entity.Requirements)
            {
                action(requirement.Key, requirement.Value);
            }
        }
    }
}