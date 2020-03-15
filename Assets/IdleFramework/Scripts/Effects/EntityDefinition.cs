using System.Collections.Generic;
using System;
using System.Numerics;
using BreakInfinity;

namespace IdleFramework
{
    public class EntityDefinition : EntityDefinitionProperties
    {
        private readonly string entityKey;
        private string name;
        private BigDouble startingQuantity;
        private ISet<string> types;
        private Dictionary<string, PropertyReference> upkeep = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> inputs = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> outputs = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> requirements = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> costs = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> minimumProduction = new Dictionary<string, PropertyReference>();
        private bool scaleProduction;
        private StateMatcher hiddenMatcher;
        private StateMatcher disabledMatcher;
        public ISet<ModifierDefinition> modifiers = new HashSet<ModifierDefinition>();
        private bool canBeBought;
        private PropertyReference quantityCap;

        public string EntityKey => entityKey;
        public string Name => name;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, PropertyReference> BaseRequirements => requirements;
        public Dictionary<string, PropertyReference> BaseCosts => costs;
        public Dictionary<string, PropertyReference> BaseUpkeep => upkeep;
        public Dictionary<string, PropertyReference> BaseProductionInputs => inputs;
        public Dictionary<string, PropertyReference> BaseProductionOutputs => outputs;
        public ISet<string> Types => types;
        public bool ScaleProductionOnAvailableInputs => scaleProduction;
        public StateMatcher HiddenMatcher => hiddenMatcher;
        public StateMatcher DisabledMatcher => disabledMatcher;
        public ISet<ModifierDefinition> Modifiers => modifiers;
        public Dictionary<string, PropertyReference> BaseMinimumProductionOutputs => minimumProduction;
        public bool CanBeBought => canBeBought;
        public PropertyReference QuantityCap => quantityCap;

        public EntityDefinition(EntityDefinitionBuilder other)
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
            this.quantityCap = other.QuantityCap != null ? other.QuantityCap : new LiteralReference(BigDouble.PositiveInfinity);
        }

        
    }
}