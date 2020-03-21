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
            RESERVED_PROPERTY_NAMES.Add("inputs");
            RESERVED_PROPERTY_NAMES.Add("outputs");
            RESERVED_PROPERTY_NAMES.Add("upkeep");
            RESERVED_PROPERTY_NAMES.Add("quantity");
            RESERVED_PROPERTY_NAMES.Add("requirements");
            RESERVED_PROPERTY_NAMES.Add("costs");
            RESERVED_PROPERTY_NAMES.Add("actual-quantity");
        }
        private readonly string entityKey;
        private string name;
        private BigDouble startingQuantity;
        private ISet<string> types;
        private Dictionary<string, ValueContainer> upkeep = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> inputs = new Dictionary<string, ValueContainer>();
        private Dictionary<string, ValueContainer> outputs = new Dictionary<string, ValueContainer>();
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

        public string EntityKey => entityKey;
        public string Name => name;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, ValueContainer> BaseRequirements => requirements;
        public Dictionary<string, ValueContainer> BaseCosts => costs;
        public Dictionary<string, ValueContainer> BaseUpkeep => upkeep;
        public Dictionary<string, ValueContainer> BaseProductionInputs => inputs;
        public Dictionary<string, ValueContainer> BaseProductionOutputs => outputs;
        public ISet<string> Types => types;
        public bool ScaleProductionOnAvailableInputs => scaleProduction;
        public StateMatcher HiddenMatcher => hiddenMatcher;
        public StateMatcher DisabledMatcher => disabledMatcher;
        public ISet<ModifierDefinition> Modifiers => modifiers;
        public Dictionary<string, ValueContainer> BaseMinimumProductionOutputs => minimumProduction;
        public bool CanBeBought => canBeBought;
        public ValueContainer QuantityCap => quantityCap;
        public Dictionary<string, ValueContainer> CustomProperties => customProperties;


        public EntityDefinition(EntityDefinitionBuilder other):base(other.EntityKey, EntityExistsMatcher(other.EntityKey), new HashSet<EntityEffectDefinition>())
        {
            this.entityKey = other.EntityKey;
            this.name = other.Name;
            this.startingQuantity = other.StartingQuantity;
            this.types = other.Types;
            this.upkeep = other.BaseUpkeep;
            this.outputs = other.BaseProductionOutputs;
            this.inputs = other.BaseProductionInputs;
            this.requirements = other.BaseRequirements;
            this.costs = other.BaseCosts;
            this.scaleProduction = other.ScaleProductionOnAvailableInputs;
            this.hiddenMatcher = other.HiddenMatcher;
            this.modifiers = other.Modifiers;
            this.disabledMatcher = other.DisabledMatcher;
            this.minimumProduction = other.BaseMinimumProductionOutputs;
            this.canBeBought = other.CanBeBought;
            this.quantityCap = other.QuantityCap != null ? other.QuantityCap : Literal.Of(BigDouble.PositiveInfinity);
            this.customProperties = other.CustomProperties;
        }

        private static StateMatcher EntityExistsMatcher(string entityKey)
        {
            return new EntityNumberPropertyMatcher(entityKey, "quantity", Comparison.GREATER_THAN, 0);
        }

        public override string ToString()
        {
            return String.Format("Entity({0}", EntityKey);
        }
    }
}