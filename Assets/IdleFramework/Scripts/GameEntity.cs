using BreakInfinity;
using IdleFramework;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace IdleFramework
{
    public class GameEntity : Updates
    {
        private readonly IdleEngine engine;
        private readonly ISet<Updates> updateables = new HashSet<Updates>();
        private ModifiableProperty quantityCap;
        private BigDouble _quantity = 0;
        private BigDouble _progress = 0;
        private readonly EntityDefinition definition;
        private readonly ModifiableProperty quantityChangePerSecond;
        
        private readonly Dictionary<string, ModifiableProperty> costs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> productionInputs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> productionOutputs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> fixedInputs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> fixedOutputs = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> upkeep = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> minimumProduction = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, ModifiableProperty> customProperties = new Dictionary<string, ModifiableProperty>();
        private readonly SimplePropertyContainer propertyContainer = new SimplePropertyContainer.SimplePropertyContainerBuilder().Build();
        public string EntityKey => definition.EntityKey;
        public string Name => definition.Name;
        public BigDouble StartingQuantity => definition.StartingQuantity;
        public bool IsEnabled => !ShouldBeDisabled(engine);
        public Dictionary<string, ValueContainer> BaseRequirements => definition.BaseRequirements;
        public Dictionary<string, ValueContainer> BaseCosts => definition.BaseCosts;
        public Dictionary<string, ValueContainer> BaseProductionInputs => definition.BaseProductionInputs;
        public Dictionary<string, ValueContainer> BaseProductionOutputs => definition.BaseProductionOutputs;
        public Dictionary<string, ValueContainer> BaseFixedProductionInputs => definition.BaseFixedProductionInputs;
        public Dictionary<string, ValueContainer> BaseFixedProductionOutputs => definition.BaseFixedProductionOutputs;
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
        public Dictionary<string, ModifiableProperty> Inputs => productionInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, ModifiableProperty> Outputs => productionOutputs;
        /*
         * The entities and quantities which are consumed by this entity as inputs to their production.
         */
        public Dictionary<string, ModifiableProperty> FixedInputs => fixedInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, ModifiableProperty> FixedOutputs => fixedOutputs;
        public Dictionary<string, ModifiableProperty> MinimumProductionOutputs => minimumProduction;
        public ISet<ModifierDefinition> Modifiers => definition.Modifiers;
        public Dictionary<string, ValueContainer> BaseMinimumProductionOutputs => definition.BaseMinimumProductionOutputs;
        public BigDouble RealQuantity => _quantity;
        public Dictionary<string, ModifiableProperty> CustomProperties => customProperties;

        public ModifiableProperty QuantityChangePerSecond => quantityChangePerSecond;
        public bool IsAvailable => definition.AvailableWhenMatcher.Matches(engine);

        public GameEntity(EntityDefinition definition, IdleEngine engine)
        {
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            foreach(var property in definition.CustomProperties)
            {
                customProperties.Add(property.Key, new ModifiableProperty(this, property.Key, property.Value, engine));
            }
            this.engine = engine;
            quantityChangePerSecond = new ModifiableProperty(this, "QuantityChangePerSecond", Literal.Of(0), engine);

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
            return definition.AvailableWhenMatcher.Matches(engine);
        }

        internal void AddModifier(ModifierDefinition modifier)
        {
            throw new NotImplementedException();
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


        internal ValueContainer GetRawProperty(string entityProperty)
        {
            string[] tokenized = entityProperty.Split('.');
            switch (tokenized[0])
            {
                case "Name":
                    return Literal.Of(Name);
                case "Qutputs":
                    return Outputs[tokenized[1]];
                case "Inputs":
                    return Inputs[tokenized[1]];
                case "Quantity":
                    return _quantity.AsContainer();
                case "Enabled":
                    return IsEnabled.AsContainer();
                case "Upkeep":
                    return Upkeep[tokenized[1]];
                case "QuantityChangePerSecond":
                    return quantityChangePerSecond;
                default:
                    assertCustomPropertyExists(entityProperty);
                    return CustomProperties[entityProperty];
            }
        }


        public bool GetPropertyAsBoolean(string entityProperty)
        {
            return GetRawProperty(entityProperty).GetAsBoolean(engine);
        }

        public BigDouble GetPropertyAsNumber(string entityProperty)
        {
            return GetRawProperty(entityProperty).GetAsNumber(engine);
        }

        public string GetPropertyAsString(string entityProperty)
        {
            return GetRawProperty(entityProperty).GetAsString(engine);
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            
            foreach(var updateable in updateables)
            {
                updateable.Update(engine, deltaTime);
            }
        }

        public void Update(IdleEngine engine, int stage, float deltaTime)
        {
            switch(stage) {
                case 1:
                    if(definition.CalculatedQuantity != null)
                    {
                        _quantity = definition.CalculatedQuantity.GetAsNumber(engine);
                    } else
                    {
                        ChangeQuantity(QuantityChangePerSecond.GetAsNumber(engine));
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void assertCustomPropertyExists(string property)
        {
            if(!customProperties.ContainsKey(property))
            {
                throw new InvalidOperationException(String.Format("Custom property {0} not defined.", property));
            }
        }

        public void setProperty(string propertyName, ValueContainer value)
        {
            propertyContainer.SetProperty(propertyName, value);
        }
    }

    public static class GameEntityExtensions
    {
        private static readonly Logger logger = Logger.GetLogger();
        public static void ForEachProperty(this GameEntity entity, Action<string, ModifiableProperty> action)
        {
            logger.Debug(string.Format("Updating properties for entity {0}", entity.Name));
            foreach(var output in entity.Outputs)
            {
                logger.Debug(String.Format("Updating property {0}[{1}]", "outputs", output.Key));
                action(output.Key, output.Value);
            }
            foreach (var input in entity.FixedInputs)
            {
                logger.Debug(String.Format("Updating property {0}[{1}]", "fixedInputs", input.Key));
                action(input.Key, input.Value);
            }
            foreach (var output in entity.FixedOutputs)
            {
                logger.Debug(String.Format("Updating property {0}[{1}]", "fixedOutputs", output.Key));
                action(output.Key, output.Value);
            }
            foreach (var input in entity.Inputs)
            {
                logger.Debug(String.Format("Updating property {0}[{1}]", "inputs", input.Key));
                action(input.Key, input.Value);
            }
            foreach (var upkeep in entity.Upkeep)
            {
                logger.Debug(String.Format("Updating property {0}[{1}]", "upkeep", upkeep.Key));
                action(upkeep.Key, upkeep.Value);
            }
            foreach (var cost in entity.Costs)
            {
                logger.Debug(String.Format("Updating property {0}[{1}]", "costs", cost.Key));
                action(cost.Key, cost.Value);
            }
        }
    }
}