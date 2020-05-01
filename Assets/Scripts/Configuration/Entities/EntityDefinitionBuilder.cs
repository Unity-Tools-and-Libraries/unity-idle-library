using BreakInfinity;
using IdleFramework.State.Matchers;
using System;
using System.Collections.Generic;

namespace IdleFramework.Configuration
{
    public class EntityDefinitionBuilder : Builder<EntityDefinition>
    {
        protected ISet<string> types = new HashSet<string>();
        protected string entityKey;
        protected string variantKey;
        protected StringContainer name;
        protected Dictionary<string, NumberContainer> baseCosts = new Dictionary<string, NumberContainer>();
        protected Dictionary<string, NumberContainer> baseScaledInputs = new Dictionary<string, NumberContainer>();
        protected Dictionary<string, NumberContainer> baseScaledOutputs = new Dictionary<string, NumberContainer>();
        protected Dictionary<string, NumberContainer> baseFixedInputs = new Dictionary<string, NumberContainer>();
        protected Dictionary<string, NumberContainer> baseFixedOutputs = new Dictionary<string, NumberContainer>();
        protected Dictionary<string, NumberContainer> baseRequirements = new Dictionary<string, NumberContainer>();
        protected Dictionary<string, NumberContainer> baseUpkeep = new Dictionary<string, NumberContainer>();

        protected BigDouble startingQuantity = 0;
        protected StateMatcher visibleMatcher = Always.Instance;
        protected StateMatcher enabledMatcher = Always.Instance;
        protected StateMatcher availableMatcher;

        private readonly Dictionary<string, EntityDefinition> entityVariants = new Dictionary<string, EntityDefinition>();
        protected NumberContainer quantityCap;
        protected bool scaleProduction = true;
        protected readonly PropertyHolder customProperties = new PropertyHolder();

        protected readonly IList<ModifierDefinition> modifiers = new List<ModifierDefinition>();
        protected bool canBeBought = true;
        protected NumberContainer calculatedQuantity;
        protected bool accumulates = true;

        public string EntityKey { get => entityKey; set => entityKey = value; }
        public string VariantKey => variantKey;
        public StringContainer Name => name;
        public ISet<string> Types => types;
        public PropertyHolder CustomProperties => customProperties;
        public BigDouble StartingQuantity => startingQuantity;
        public Dictionary<string, NumberContainer> BaseCosts => baseCosts;
        public Dictionary<string, NumberContainer> BaseProductionInputs => baseScaledInputs;
        public Dictionary<string, NumberContainer> BaseProductionOutputs => baseScaledOutputs;
        public Dictionary<string, NumberContainer> BaseRequirements => baseRequirements;

        public EntityDefinitionBuilder WithVariant(EntityDefinitionBuilder entityInstanceBuilder)
        {
            entityInstanceBuilder.EntityKey = this.entityKey;
            entityVariants[entityInstanceBuilder.VariantKey] = new EntityDefinition(entityInstanceBuilder);
            return this;
        }

        public Dictionary<string, NumberContainer> BaseFixedProductionInputs => baseFixedInputs;
        public Dictionary<string, NumberContainer> BaseFixedProductionOutputs => baseFixedOutputs;
        public Dictionary<string, NumberContainer> BaseUpkeep => baseUpkeep;
        public bool ScaleProductionOnAvailableInputs => scaleProduction;
        public StateMatcher IsVisibleMatcher => visibleMatcher;
        public StateMatcher IsEnabledMatcher => enabledMatcher;
        public StateMatcher IsAvailableMatcher => availableMatcher;
        public bool CanBeBought => canBeBought;
        public NumberContainer QuantityCap => quantityCap;
        public NumberContainer CalculatedQuantity => calculatedQuantity;
        public Dictionary<string, EntityDefinition> Variants => entityVariants;
        public bool Accumulates => accumulates;
        public IList<ModifierDefinition> Modifiers => modifiers;

        /*
         * Create a new EntityDefinitionBuilder, for an entity that will have the given key.
         */
        public EntityDefinitionBuilder(string entityKey, string variantKey)
        {
            this.entityKey = entityKey;
            this.variantKey = variantKey;
            name = Literal.Of(entityKey);
            quantityCap = Literal.Of(BigDouble.PositiveInfinity);
            this.availableMatcher = new DelegateStateMatcher(engine =>
            {
                var entity = engine.GetEntity(entityKey);
                if(!entity.IsEnabled)
                {
                    return false;
                }
                if(entity.Quantity >= entity.QuantityCap)
                {
                    return false;
                }
                bool available = true;
                foreach(var resource in entity.Costs)
                {
                    available = engine.GetEntity(resource.Key).Quantity >= resource.Value;
                    if(!available)
                    {
                        return false;
                    }
                }
                foreach (var resource in entity.Requirements)
                {
                    available = engine.GetEntity(resource.Key).Quantity >= resource.Value;
                    if (!available)
                    {
                        return false;
                    }
                }
                return available;
            });
        }
        public EntityDefinitionBuilder(string entityKey) : this(entityKey, entityKey)
        {

        }
        public EntityDefinitionBuilder LimitOne()
        {
            quantityCap = Literal.Of(1);
            return this;
        }

        public EntityDefinitionBuilder DoesNotAccumulate()
        {
            accumulates = false;
            return this;
        }

        /*
         * Set the displayed name of the entity.
         */
        public EntityDefinitionBuilder WithName(StringContainer name)
        {
            this.name = name;
            return this;
        }

        public EntityDefinitionBuilder WithName(string name)
        {
            return WithName(Literal.Of(name));
        }

        public EntityDefinitionBuilder AvailableWhen(StateMatcher matcher)
        {
            this.availableMatcher = matcher;
            return this;
        }

        /*
         * Add a cost to the entity. These costs are entities which are lost when purchasing new instances of the entity.
         */ 
        public EntityDefinitionBuilder WithCost(string entityRequired, BigDouble quantityRequired)
        {
            baseCosts.Add(entityRequired, Literal.Of(quantityRequired));
            return this;
        }

        public EntityDefinitionBuilder WithCost(string entityRequired, NumberContainer quantityRequired)
        {
            baseCosts.Add(entityRequired, quantityRequired);
            return this;
        }

        /*
         * Add a production input to the entity. These inputs are consumed when the entity generates output and constrains output.
         */
        public EntityDefinitionBuilder WithScaledInput(string entity, NumberContainer quantityConsumedPerTick)
        {
            baseScaledInputs[entity] = quantityConsumedPerTick;
            return this;
        }

        public EntityDefinitionBuilder WithScaledInput(string entity, BigDouble quantityConsumedPerTick)
        {
            baseScaledInputs[entity] = Literal.Of(quantityConsumedPerTick);
            return this;
        }

        public EntityDefinitionBuilder WithFixedOutput(string entityKey, NumberContainer quantity)
        {
            baseFixedOutputs[entityKey] = quantity;
            return this;
        }

        public EntityDefinitionBuilder WithFixedOutput(string entityKey, BigDouble quantity)
        {
            baseFixedOutputs[entityKey] = Literal.Of(quantity);
            return this;
        }


        public EntityDefinitionBuilder WithFixedInput(string entityKey, NumberContainer quantity)
        {
            baseFixedInputs[entityKey] = quantity;
            return this;
        }

        public EntityDefinitionBuilder WithFixedInput(string entityKey, BigDouble quantity)
        {
            baseFixedInputs[entityKey] = Literal.Of(quantity);
            return this;
        }

        /*
         * Add a production output to the entity. These outputs are generated by the entity.
         * 
         * The quantity specified is a fixed amount.
         */
        public EntityDefinitionBuilder WithScaledOutput(string entityKey, NumberContainer quantityPerTick)
        {
            baseScaledOutputs[entityKey] = quantityPerTick;
            return this;
        }

        public EntityDefinitionBuilder WithScaledOutput(string entityKey, BigDouble quantityPerTick)
        {
            baseScaledOutputs[entityKey] = Literal.Of(quantityPerTick);
            return this;
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
        public EntityDefinitionBuilder HiddenWhen(StateMatcher matcher)
        {
            visibleMatcher = matcher.Negate();
            return this;
        }

        /*
         * Returns a configurer for configuring when the entity is inactive.
         */
        public EntityDefinitionBuilder DisabledWhen(StateMatcher matcher)
        {
            enabledMatcher = matcher.Negate();
            return this;
        }

        /*
         * Sets the entity as unpurchaseable by the user.
         */
        public EntityDefinitionBuilder UnbuyableWhen(StateMatcher matcher)
        {
            availableMatcher = matcher.Negate();
            return this;
        }
        public EntityDefinitionBuilder QuantityCappedBy(NumberContainer entityValueContainer)
        {
            quantityCap = entityValueContainer;
            return this;
        }

        public EntityDefinitionBuilder WithCalculatedQuantity(NumberContainer value)
        {
            this.calculatedQuantity = value;
            return this;
        }

        public EntityDefinitionBuilder HiddenAndDisabledWhen(StateMatcher matcher)
        {
            visibleMatcher = matcher.Negate();
            enabledMatcher = matcher.Negate();
            return this;
        }
        public EntityDefinitionBuilder WithCustomBooleanProperty(string propertyName, bool value)
        {
            customProperties.Set(propertyName, Literal.Of(value));
            return this;
        }
        public EntityDefinitionBuilder WithCustomBooleanProperty(string propertyName, BooleanContainer value)
        {
            customProperties.Set(propertyName, value);
            return this;
        }
        public EntityDefinitionBuilder WithCustomStringProperty(string propertyName)
        {
            return WithCustomStringProperty(propertyName, Literal.Of(""));
        }
        public EntityDefinitionBuilder WithCustomStringProperty(string propertyName, StringContainer value)
        {
            customProperties.Set(propertyName, value);
            return this;
        }
        public EntityDefinitionBuilder WithCustomNumberProperty(string propertyName, NumberContainer value)
        {
            customProperties.Set(propertyName, value);
            return this;
        }
        public EntityDefinitionBuilder WithCustomNumberProperty(string propertyName)
        {
            return WithCustomNumberProperty(propertyName, Literal.Of(0));
        }
        public EntityDefinitionBuilder WithCustomMapProperty(string propertyName)
        {
            customProperties.Set(propertyName, (MapContainer)null);
            return this;
        }

        public EntityDefinitionBuilder WithCustomMapProperty(string propertyName, Dictionary<string, ValueContainer> value)
        {
            customProperties.Set(propertyName, Literal.Of(value));
            return this;
        }

        public EntityDefinitionBuilder WithEffect(EntityModifierDefinition entityPropertyModifier)
        {
            modifiers.Add(entityPropertyModifier);
            return this;
        }
    }
}