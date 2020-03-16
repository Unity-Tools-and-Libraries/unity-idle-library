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
        private StateMatcher hideEntityMatcher = Never.Instance;
        private StateMatcher disabledWhenMatcher = Never.Instance;
        private ISet<ModifierDefinition> modifiers = new HashSet<ModifierDefinition>();
        private PropertyReference quantityCap;
        private bool scaleProduction = true;
        private readonly Dictionary<string, PropertyReference> customProperties = new Dictionary<string, PropertyReference>();

        public EntityDefinitionBuilder QuantityCappedBy(PropertyReference entityPropertyReference)
        {
            quantityCap = entityPropertyReference;
            return this;
        }

        public HiddenAndDisabledConfigurationBuilder HiddenAndDisabled()
        {
            return new HiddenAndDisabledConfigurationBuilder(this);
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

        public Dictionary<string, PropertyReference> CustomProperties => customProperties;

        /*
         * Create a new EntityDefinitionBuilder, for an entity that will have the given key.
         */
        public EntityDefinitionBuilder(string key)
        {
            this.key = key;
        }

        /*
         * Specify the minimum production that the entity will have while active.
         * 
         * Unlike production defined in ProductionOutputs, this minimum production ignores entity quantity.
         */
        public EntityDefinitionBuilder WithFlatMinimumProduction(string entity, BigDouble value)
        {
            minimumProductionOutputs[entity] = Literal.Of(value);
            return this;
        }

        /*
         * Set the displayed name of the entity.
         */
        public EntityDefinitionBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        /*
         * Add a requirement to the entity. These requirements must be met to purchase new instances of the entity.
         */
        public EntityDefinitionBuilder WithRequirement(string entityRequired, BigDouble quantityRequired)
        {
            requires.Add(entityRequired, Literal.Of(quantityRequired));
            return this;
        }

        /*
         * Add a cost to the entity. These costs are entities which are lost when purchasing new instances of the entity.
         */ 
        public EntityDefinitionBuilder WithCost(string entityRequired, BigDouble quantityRequired)
        {
            costs.Add(entityRequired, Literal.Of(quantityRequired));
            return this;
        }

        /*
         * Add a production input to the entity. These inputs are consumed when the entity generates output and constrains output.
         * 
         * The quantity specified is a fixed amount per entity.
         */
        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick)
        {
            productionInputs[entity] = Literal.Of(quantityConsumedPerTick);
            return this;
        }

        /*
         * Add a production input to the entity. These inputs are consumed when the entity generates output and constrains output.
         * 
         * The quantity specified is a fixed amount per entity.
         */
        public EntityDefinitionBuilder WithConsumption(string entity, BigDouble quantityConsumedPerTick, PropertyReference cap)
        {
            productionInputs[entity] = Min.Of(Literal.Of(quantityConsumedPerTick), cap);
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is a fixed amount.
         */
        public EntityDefinitionBuilder WithProduction(string entityKey, BigDouble quantityPerTick)
        {
            productionOutputs[entityKey] = Literal.Of(quantityPerTick);
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is based on one or more other properties.
         */
        public EntityDefinitionBuilder WithProduction(string entityKey, PropertyReference quantityPerTick)
        {
            productionOutputs[entityKey] = quantityPerTick;
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is based on some other value and is capped.
         */ 
        public EntityDefinitionBuilder WithProduction(string entityKey, PropertyReference quantityPerTick, EntityPropertyReference cap)
        {
            return WithProduction(entityKey, Min.Of(quantityPerTick, cap));
        }

        /*
         * Add a type tag to the entity.
         */
        public EntityDefinitionBuilder WithType(string type)
        {
            types.Add(type);
            return this;
        }

        /*
         * Specify the quantity of the entity that is present at the start of the game.
         */
        public EntityDefinitionBuilder WithStartingQuantity(BigDouble startingQuantity)
        {
            this.startingQuantity = startingQuantity;
            return this;
        }
        /*
         * Add an upkeep requirement to the entity. These upkeep values are consumed for each entity each tick and a shortfall causes the entity quantity to be reduced.
         */
        public EntityDefinitionBuilder WithUpkeepRequirement(string entity, BigDouble quantity)
        {
            BaseUpkeep.Add(entity, Literal.Of(quantity));
            return this;
        }

        /*
         * Complete the configuration of the entity, returning the final EntityDefinition.
         */
        public EntityDefinition Build()
        {
            return new EntityDefinition(this);
        }

        /*
         * Returns a configurer for configuring when the entity should be hidden from the user.
         */
        public EntityHideConfigurationBuilder Hidden()
        {
            return new EntityHideConfigurationBuilder(this);
        }

        /*
         * Returns a configurer for configuring when the entity is inactive.
         */
        public EntityDisabledConfigurationBuilder Disabled()
        {
            return new EntityDisabledConfigurationBuilder(this);
        }

        /*
         * Sets the entity as unpurchaseable by the user.
         */
        public EntityDefinitionBuilder Unbuyable()
        {
            this.canBeBought = false;
            return this;
        }

        public EntityDefinitionBuilder WithCustomProperty(string customProperty)
        {
            customProperties.Add(customProperty, Literal.Of(0));
            return this;
        }

        /*
         * Class for configuring when the parent entity should be hidden from the user.
         */
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
            /*
             * Sets this entity as always hidden.
             */
            public EntityHideConfigurationBuilder Always()
            {
                parent.hideEntityMatcher = IdleFramework.Always.Instance;
                return this;
            }

            /*
             * Return the parent to further configure it.
             */
            public EntityDefinitionBuilder And()
            {
                return parent;
            }
            /*
             * Return the parent to further configure it.
             * /
            public EntityDefinitionBuilder Done()
            {
                return parent;
            }
            /*
             * Add a state matcher to determine when the entity should be hidden.
             */
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

        public class HiddenAndDisabledConfigurationBuilder
        {
            private EntityDefinitionBuilder parent;

            public HiddenAndDisabledConfigurationBuilder(EntityDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public HiddenAndDisabledConfigurationBuilder When(StateMatcher matcher)
            {
                parent.hideEntityMatcher = matcher;
                parent.disabledWhenMatcher = matcher;
                return this;
            }

            public EntityDefinitionBuilder Done()
            {
                return parent;
            }
        }
    }

    public interface EntityDefinitionProperties
    {
        /*
         * Get all types that this entity is tagged with.
         */
        ISet<string> Types { get; }
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
        { get; }
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

        ISet<ModifierDefinition> Modifiers { get; }

        bool CanBeBought { get; }

        PropertyReference QuantityCap { get; }

        Dictionary<string, PropertyReference> CustomProperties { get; }
    }    
}