using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class EntityDefinitionBuilder : EntityDefinitionProperties
    {
        private ISet<string> types = new HashSet<string>();
        private string key;
        private string name;
        private Dictionary<string, PropertyReference> requires = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> costs = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> productionInputs = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> productionOutputs = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> upkeep = new Dictionary<string, PropertyReference>();
        private Dictionary<string, PropertyReference> minimumProductionOutputs = new Dictionary<string, PropertyReference>();
        private BigDouble startingQuantity = 0;
        private bool scaleProduction;
        private StateMatcher hideEntityMatcher = new Never();
        private StateMatcher disabledWhenMatcher = new Never();
        private ISet<ModifierDefinition> modifiers = new HashSet<ModifierDefinition>();
        private PropertyReference quantityCap;

        public EntityDefinitionBuilder QuantityCappedBy(EntityPropertyReference entityPropertyReference)
        {
            quantityCap = entityPropertyReference;
            return this;
        }

        private bool canBeBought = true;

        

        public string EntityKey => key;
        public string Name => name;
        public ISet<string> Types => types;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, PropertyReference> BaseRequirements => requires;
        public Dictionary<string, PropertyReference> BaseCosts => costs;
        public Dictionary<string, PropertyReference> BaseProductionInputs => productionInputs;
        public Dictionary<string, PropertyReference> BaseProductionOutputs => productionOutputs;
        public Dictionary<string, PropertyReference> BaseUpkeep => upkeep;
        public Dictionary<string, PropertyReference> BaseMinimumProductionOutputs => minimumProductionOutputs;
        public bool ScaleProductionOnAvailableInputs => scaleProduction;
        public StateMatcher HiddenMatcher => hideEntityMatcher;
        public StateMatcher DisabledMatcher => disabledWhenMatcher;
        public ISet<ModifierDefinition> Modifiers => modifiers;
        public bool CanBeBought => canBeBought;
        public PropertyReference QuantityCap => quantityCap;

        public EntityDefinitionBuilder(string key)
        {
            this.key = key;
        }

        public EntityDefinitionBuilder WithMinimumProduction(string entity, int value)
        {
            minimumProductionOutputs[entity] = new LiteralReference(value);
            return this;
        }

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
            requires.Add(entityRequired, new LiteralReference(quantityRequired));
            return this;
        }

        public EntityDefinitionBuilder WithModifier(ModifierDefinition modifier)
        {
            modifiers.Add(modifier);
            return this;
        }

        public EntityDefinitionBuilder WithCost(string entityRequired, BigDouble quantityRequired)
        {
            costs.Add(entityRequired, new LiteralReference(quantityRequired));
            return this;
        }

        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick)
        {
            productionInputs[entity] = new LiteralReference(quantityConsumedPerTick);
            return this;
        }

        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick, PropertyReference cap)
        {
            productionInputs[entity] = new MinOf(new LiteralReference(quantityConsumedPerTick), cap);
            return this;
        }

        public EntityDefinitionBuilder WithProduction(string entityKey, BigDouble progressAddedPerTick)
        {
            productionOutputs[entityKey] = new LiteralReference(progressAddedPerTick);
            return this;
        }

        public EntityDefinitionBuilder WithProduction(string entityKey, PropertyReference progressAddedPerTick)
        {
            productionOutputs[entityKey] = progressAddedPerTick;
            return this;
        }

        public EntityDefinitionBuilder WithProduction(string entityKey, PropertyReference progressPerTick, EntityPropertyReference cap)
        {
            return WithProduction(entityKey, new MinOf(progressPerTick, cap));
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

        public EntityDefinitionBuilder WithUpkeepRequirement(string entity, BigDouble quantity)
        {
            BaseUpkeep.Add(entity, new LiteralReference(quantity));
            return this;
        }

        public EntityDefinition Build()
        {
            return new EntityDefinition(this);
        }

        public EntityHideConfigurationBuilder Hidden()
        {
            return new EntityHideConfigurationBuilder(this);
        }


        public EntityDisabledConfigurationBuilder Disabled()
        {
            return new EntityDisabledConfigurationBuilder(this);
        }

        public EntityDefinitionBuilder Unbuyable()
        {
            this.canBeBought = false;
            return this;
        }

        public class EntityHideConfigurationBuilder
        {
            EntityDefinitionBuilder parent;

            public EntityHideConfigurationBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public ISet<string> Types => ((EntityDefinitionProperties)parent).Types;

            public string EntityKey => ((EntityDefinitionProperties)parent).EntityKey;

            public string Name => ((EntityDefinitionProperties)parent).Name;

            public BigDouble StartingQuantity => ((EntityDefinitionProperties)parent).StartingQuantity;

            public Dictionary<string, PropertyReference> BaseRequirements => ((EntityDefinitionProperties)parent).BaseRequirements;

            public Dictionary<string, PropertyReference> BaseCosts => ((EntityDefinitionProperties)parent).BaseCosts;

            public Dictionary<string, PropertyReference> BaseUpkeep => ((EntityDefinitionProperties)parent).BaseUpkeep;

            public Dictionary<string, PropertyReference> BaseProductionInputs => ((EntityDefinitionProperties)parent).BaseProductionInputs;

            public Dictionary<string, PropertyReference> BaseProductionOutputs => ((EntityDefinitionProperties)parent).BaseProductionOutputs;

            public bool ScaleProductionOnAvailableInputs => ((EntityDefinitionProperties)parent).ScaleProductionOnAvailableInputs;

            public StateMatcher HiddenMatcher => ((EntityDefinitionProperties)parent).HiddenMatcher;

            public StateMatcher DisabledMatcher => ((EntityDefinitionProperties)parent).DisabledMatcher;

            public EntityHideConfigurationBuilder Always()
            {
                parent.hideEntityMatcher = new Always();
                return this;
            }

            public EntityDefinitionBuilder And()
            {
                return parent;
            }

            public EntityDefinitionBuilder Done()
            {
                return parent;
            }

            public EntityHideConfigurationBuilder When(EntityPropertyMatcher entityPropertyMatcher)
            {
                parent.hideEntityMatcher = entityPropertyMatcher;
                return this;
            }
        }

        public class EntityDisabledConfigurationBuilder
        {
            private EntityDefinitionBuilder parent;

            public EntityDisabledConfigurationBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public EntityDefinitionBuilder Done()
            {
                return parent;
            }

            public EntityDisabledConfigurationBuilder When(EntityPropertyMatcher entityPropertyMatcher)
            {
                parent.disabledWhenMatcher = entityPropertyMatcher;
                return this;
            }

            public EntityDefinition Build()
            {
                return parent.Build();
            }
        }

        public class EntityModifierDefinitionBuilder
        {
            private EntityDefinitionBuilder parent;

            public EntityModifierDefinitionBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public EntityModifierDefinitionBuilder Active()
            {
                return this;
            }

            public EntityDefinition Build()
            {
                return parent.Build();
            }
        }
    }

    public interface EntityDefinitionProperties
    {
        /*
         * Get all types that this entity is tagged with.
         */
        ISet<string> Types { get;  }
        /*
         * The unique key identifying this entity.
         */
        string EntityKey { get; }
        /*
         * The displayable name of this entity.
         */
        string Name { get; }
        /*
         * The quantity of this entity that is present at the beginning of a new game.
         */
        BigDouble StartingQuantity
        { get;}
        /*
         * The unmodified quantities of entities which are required when trying to buy this entity.
         */
        Dictionary<string, PropertyReference> BaseRequirements { get; }
        /*
         * The unmodified entities and quantities which are consumed to buy this entity.
         */
        Dictionary<string, PropertyReference> BaseCosts { get; }
        /*
         * The unmodified entities and quantities which are consumed each tick by this entity and if a shortfall of these requirements causes the loss of this entity.
         */
        Dictionary<string, PropertyReference> BaseUpkeep { get; }
        /*
         * The unmodified entities and quantities which are consumed by this entity as inputs to their production.
         */
        Dictionary<string, PropertyReference> BaseProductionInputs { get; }
        /*
         * The unmodified entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        Dictionary<string, PropertyReference> BaseProductionOutputs { get; }
        /*
         * The unmodified minimum entities and quantities that this entity produces each tick, independent of the quantity
         */
        Dictionary<string, PropertyReference> BaseMinimumProductionOutputs { get; }
        /*
         * If the quantities this entity produces are scaled by the proportion of available inputs.
         * 
         * When true, both production and consumption are scaled by the available portion of inputs.
         * 
         * When false, if less than 100% of inputs are available then no inputs are consumed and no outputs are produced.
         */
        bool ScaleProductionOnAvailableInputs { get; }

        StateMatcher HiddenMatcher { get; }

        StateMatcher DisabledMatcher { get; }

        ISet<ModifierDefinition> Modifiers { get;  }

        bool CanBeBought { get; }

        PropertyReference QuantityCap { get; }
    }    
}