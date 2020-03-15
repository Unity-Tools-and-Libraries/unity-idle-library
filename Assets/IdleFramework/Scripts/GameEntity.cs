using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    public class GameEntity : EntityDefinitionProperties
    {
        private readonly IdleEngine engine;
        private BigDouble quantityCap;
        private BigDouble _quantity = 0;
        private BigDouble _progress = 0;
        public readonly EntityDefinition definition;
        private readonly Dictionary<string, GameEntityProperty> requirements = new Dictionary<string, GameEntityProperty>();
        private readonly Dictionary<string, GameEntityProperty> costs = new Dictionary<string, GameEntityProperty>();
        private readonly Dictionary<string, GameEntityProperty> productionInputs = new Dictionary<string, GameEntityProperty>();
        private readonly Dictionary<string, GameEntityProperty> productionOutputs = new Dictionary<string, GameEntityProperty>();
        private readonly Dictionary<string, GameEntityProperty> upkeep = new Dictionary<string, GameEntityProperty>();
        private readonly Dictionary<string, GameEntityProperty> minimumProduction = new Dictionary<string, GameEntityProperty>();
        private readonly Dictionary<string, GameEntityProperty> customProperties = new Dictionary<string, GameEntityProperty>();

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
                return BigDouble.Min(actualQuantity, cap);
           }
        }
        public BigDouble Progress => _progress;
        public ISet<string> Types => definition.Types;
        public bool ScaleProductionOnAvailableInputs => definition.ScaleProductionOnAvailableInputs;
        public StateMatcher HiddenMatcher => definition.HiddenMatcher;
        public StateMatcher DisabledMatcher => definition.DisabledMatcher;
        public PropertyReference QuantityCap => definition.QuantityCap;

        /*
         * The quantities of entities which are required when trying to buy this entity.
         */
        public Dictionary<string, GameEntityProperty> Requirements => requirements;
        /*
         * The entities and quantities which are consumed to buy this entity.
         */
        public Dictionary<string, GameEntityProperty> Costs => costs;
        /*
         * The entities and quantities which are consumed each tick by this entity and if a shortfall of these requirements causes the loss of this entity.
         */
        public Dictionary<string, GameEntityProperty> Upkeep => upkeep;
        /*
         * The entities and quantities which are consumed by this entity as inputs to their production.
         */
        public Dictionary<string, GameEntityProperty> ProductionInputs => productionInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, GameEntityProperty> ProductionOutputs => productionOutputs;
        public Dictionary<string, GameEntityProperty> MinimumProductionOutputs => minimumProduction;
        public ISet<ModifierDefinition> Modifiers => ((EntityDefinitionProperties)definition).Modifiers;
        public Dictionary<string, PropertyReference> BaseMinimumProductionOutputs => definition.BaseMinimumProductionOutputs;
        public bool CanBeBought => definition.CanBeBought;
        public BigDouble RealQuantity => _quantity;

        public Dictionary<string, PropertyReference> CustomProperties => definition.CustomProperties;

        public GameEntity(EntityDefinition definition, IdleEngine engine)
        {
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            foreach(var property in definition.CustomProperties)
            {
                customProperties.Add(property.Key, new GameEntityProperty(0));
            }
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
                requirementsMet = engine.AllEntities[requirement.Key].Quantity >= requirement.Value.Value;
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
                var quantityWithSufficientInputs = BigDouble.Min(engine.AllEntities[requirement.Key].Quantity / requirement.Value.Value, this.Quantity);
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
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
        }

        public void SetQuantity(BigDouble newQuantity)
        {
            _quantity = newQuantity;
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
        }

        public void ChangeProgress(BigDouble changeBy)
        {
            _progress += changeBy;
            if (_progress >= 1)
            {
                _progress = 0;
                _quantity += 1;
            }
            _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
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

        public bool HasCustomProperty(string customProperty)
        {
            return customProperties.ContainsKey(customProperty);
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

    public class GameEntityProperty
    {
        private BigDouble value;
        private List<ModifierAndEffect> appliedModifiers = new List<ModifierAndEffect>();

        public GameEntityProperty(BigDouble quantity, params ModifierAndEffect[] initialModifiers)
        {
            this.value = quantity != null ? quantity : default(BigDouble);
            AppliedModifiers.AddRange(initialModifiers);
        }

        public BigDouble Value { get => value; set {
                this.value = value != null ? value : default(BigDouble);
            } }


        public List<ModifierAndEffect> AppliedModifiers { get => appliedModifiers; set => appliedModifiers = value; }

        public static implicit operator GameEntityProperty(BigDouble bigDouble) => new GameEntityProperty(bigDouble);
        public static implicit operator BigDouble(GameEntityProperty gep) => gep.value;

        public static bool operator ==(GameEntityProperty left, GameEntityProperty right) => left.value.Equals(right.value);
        public static bool operator ==(GameEntityProperty left, BigDouble right) => left.value.Equals(right);

        public static bool operator !=(GameEntityProperty left, GameEntityProperty right) => !left.value.Equals(right.value);
        public static bool operator !=(GameEntityProperty left, BigDouble right) => !left.value.Equals(right);

        public static BigDouble operator -(GameEntityProperty operand) => -operand.value;
        public static BigDouble operator +(GameEntityProperty left, GameEntityProperty right) => left.value + right.value;

        public static BigDouble operator /(GameEntityProperty left, GameEntityProperty right) => left.value / right.value;
        public static BigDouble operator /(BigDouble left, GameEntityProperty right) => left / right.value;
        public static BigDouble operator /(GameEntityProperty left, BigDouble right) => left.value / right;

        public override bool Equals(object obj)
        {
            return obj is GameEntityProperty property &&
                   value.Equals(property.value) &&
                   EqualityComparer<List<ModifierAndEffect>>.Default.Equals(appliedModifiers, property.appliedModifiers);
        }

        public override int GetHashCode()
        {
            var hashCode = 1254980278;
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ModifierAndEffect>>.Default.GetHashCode(appliedModifiers);
            return hashCode;
        }
    }

    public struct ModifierAndEffect
    {
        public readonly ModifierDefinition modifier;
        public readonly EntityEffect effect;

        public ModifierAndEffect(ModifierDefinition modifier, EntityEffect effect)
        {
            this.modifier = modifier;
            this.effect = effect;
        }
    }
}