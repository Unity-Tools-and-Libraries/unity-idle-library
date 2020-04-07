using System.Collections.Generic;
using System;
using System.Numerics;
using BreakInfinity;

namespace IdleFramework
{
    public class EntityDefinition : ModifierDefinition
    {
        public static readonly ISet<string> RESERVED_PROPERTY_NAMES = new HashSet<string>();
        static EntityDefinition()
        {
            RESERVED_PROPERTY_NAMES.Add("Inputs");
            RESERVED_PROPERTY_NAMES.Add("Outputs");
            RESERVED_PROPERTY_NAMES.Add("Upkeep");
            RESERVED_PROPERTY_NAMES.Add("Quantity");
            RESERVED_PROPERTY_NAMES.Add("Requirements");
            RESERVED_PROPERTY_NAMES.Add("Costs");
            RESERVED_PROPERTY_NAMES.Add("Actual-quantity");
        }
        private readonly string entityKey;
        private string name;
        private ValueContainer calculatedQuantity;
        private BigDouble startingQuantity;
        private ISet<string> types;
        private Dictionary<string, ValueContainer> upkeep = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> inputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> outputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> fixedInputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> fixedOutputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> requirements = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> costs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> minimumProduction = new Dictionary<string, ValueContainer>();
        private bool scaleProduction;
        private StateMatcher hiddenMatcher;
        private StateMatcher disabledMatcher;
        public ISet<ModifierDefinition> modifiers = new HashSet<ModifierDefinition>();
        private bool canBeBought;
        private ValueContainer quantityCap;
        private readonly Dictionary<string, ValueContainer> customProperties;
        private readonly StateMatcher availableWhenMatcher;

        public string EntityKey => entityKey;
        public string Name => name != null ? name : entityKey;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, ValueContainer> BaseRequirements => requirements;
        public Dictionary<string, ValueContainer> BaseCosts => costs;
        public Dictionary<string, ValueContainer> BaseUpkeep => upkeep;
        public Dictionary<string, ValueContainer> BaseProductionInputs => inputs;
        public Dictionary<string, ValueContainer> BaseProductionOutputs => outputs;
        public Dictionary<string, ValueContainer> BaseFixedProductionInputs => fixedInputs;
        public Dictionary<string, ValueContainer> BaseFixedProductionOutputs => fixedOutputs;
        public ISet<string> Types => types;
        public bool ScaleProductionOnAvailableInputs => scaleProduction;
        public StateMatcher HiddenMatcher => hiddenMatcher;
        public StateMatcher DisabledMatcher => disabledMatcher;
        public ISet<ModifierDefinition> Modifiers => modifiers;
        public Dictionary<string, ValueContainer> BaseMinimumProductionOutputs => minimumProduction;
        public ValueContainer QuantityCap => quantityCap;
        public Dictionary<string, ValueContainer> CustomProperties => customProperties;

        public StateMatcher AvailableWhenMatcher => availableWhenMatcher;
        public ValueContainer CalculatedQuantity => calculatedQuantity;

        public EntityDefinition(EntityDefinitionBuilder other):base(other.EntityKey, EntityExistsMatcher(other.EntityKey), new HashSet<EntityEffectDefinition>())
        {
            this.entityKey = other.EntityKey;
            this.name = other.Name;
            this.startingQuantity = other.StartingQuantity;
            this.types = other.Types;
            this.upkeep = other.BaseUpkeep;
            this.outputs = other.BaseProductionOutputs;
            this.inputs = other.BaseProductionInputs;
            this.fixedOutputs = other.BaseFixedProductionOutputs;
            this.fixedInputs = other.BaseFixedProductionInputs;
            this.costs = other.BaseCosts;
            this.scaleProduction = other.ScaleProductionOnAvailableInputs;
            this.hiddenMatcher = other.HiddenMatcher;
            this.modifiers = other.Modifiers;
            this.disabledMatcher = other.DisabledMatcher;
            this.minimumProduction = other.BaseMinimumProductionOutputs;
            this.canBeBought = other.CanBeBought;
            this.quantityCap = other.QuantityCap != null ? other.QuantityCap : Literal.Of(BigDouble.PositiveInfinity);
            this.calculatedQuantity = other.CalculatedQuantity;
            this.customProperties = other.CustomProperties;
            this.availableWhenMatcher = other.AvailableMatcher;
        }

        private static StateMatcher EntityExistsMatcher(string entityKey)
        {
            return new EntityNumberPropertyMatcher(entityKey, "Quantity", Comparison.GREATER_THAN, 0);
        }

        public override string ToString()
        {
            return String.Format("Entity({0})", EntityKey);
        }
    }
}