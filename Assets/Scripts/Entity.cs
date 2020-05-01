using BreakInfinity;
using IdleFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using UnityEngine;

namespace IdleFramework
{
    public class Entity : Updates, IEntityProperties, CanSnapshot<EntitySnapshot>
    {
        public static readonly IDictionary<string, string> PREDEFINED_PROPERTIES = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
        {
            { "Name", "string" },
            { "Quantity", "number" },
            { "QuantityCap", "number" },
            { "QuantityBought", "number" },
            { "Costs", "map" },
            { "ScaledInputs", "map" },
            { "ScaledOutputs", "map" },
            { "Upkeep", "map" },
            { "MinimumProduction", "map" },
            { "QuantityChangePerSecond", "number" },
            { "Progress", "number" },
            { "Enabled", "boolean" },
            { "HighestQuantityAchieved", "number" }
        });

        public static readonly IDictionary<string, ISet<string>> PREDEFINED_PROPERTY_TYPES = new ReadOnlyDictionary<string, ISet<string>>(new Dictionary<string, ISet<string>>() {
            { "number", new HashSet<string>()
            {
                "Quantity",
                "QuantityCap",
                "QuantityBought",
                "QuantityChangePerSecond",
                "HighestQuantityAchieved",
                "Progress"
            } },
            { "map", new HashSet<string>()
            {
                "ScaledInputs",
                "ScaledOutputs",
                "FixedInputs",
                "FixedOutputs",
                "Upkeep",
            } },
            { "boolean", new HashSet<string>()
            {
                "Available",
                "Enabled",
                "Visible"
            } },
            { "string", new HashSet<string>()
            {
                "Name"
            } }
        });
        protected readonly IdleEngine engine;
        protected NumberContainer quantityCap;
        protected BigDouble _quantity = 0;
        protected BigDouble _progress = 0;
        protected readonly IEntityDefinition definition;
        protected BigDouble quantityChangePerSecond;
        protected BigDouble quantityBought;
        protected BigDouble highestQuantityAchieved;

        protected readonly Dictionary<string, BigDouble> costs = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, BigDouble> requirements = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, BigDouble> scaledInputs = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, BigDouble> scaledOutputs = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, BigDouble> fixedInputs = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, BigDouble> fixedOutputs = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, BigDouble> upkeep = new Dictionary<string, BigDouble>();
        protected readonly Dictionary<string, Entity> variants = new Dictionary<string, Entity>();
        protected readonly PropertyHolder customProperties = new PropertyHolder();
        protected readonly IList<Modifier> modifiers = new List<Modifier>();
        public string EntityKey => definition.EntityKey;
        public string VariantKey => definition.VariantKey;
        public string Name => definition.Name.Get(engine);
        public BigDouble StartingQuantity => definition.StartingQuantity;
        public Dictionary<string, NumberContainer> BaseRequirements => definition.BaseRequirements;
        public Dictionary<string, NumberContainer> BaseCosts => definition.BaseCosts;
        public Dictionary<string, NumberContainer> BaseScaledInputs => definition.BaseScaledInputs;
        public Dictionary<string, NumberContainer> BaseScaledOutputs => definition.BaseScaledOutputs;
        public Dictionary<string, NumberContainer> BaseFixedInputs => definition.BaseFixedInputs;
        public Dictionary<string, NumberContainer> BaseFixedOutputs => definition.BaseFixedOutputs;
        public Dictionary<string, NumberContainer> BaseUpkeep => definition.BaseUpkeep;
        public bool Accumulates => definition.Accumulates;
        public BigDouble Quantity
        {
            get
            {
                var actualQuantity = _quantity;
                var cap = QuantityCap != null ? QuantityCap : _quantity;
                return BigDouble.Min(actualQuantity, cap);
            }
        }
        public BigDouble Progress => _progress;
        public ISet<string> Types => definition.Types;
        public StateMatcher HiddenMatcher => definition.IsVisibleMatcher;
        public StateMatcher EnabledMatcher => definition.IsEnabledMatcher;
        public BigDouble QuantityCap => definition.QuantityCap.Get(engine);

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
        public Dictionary<string, BigDouble> ScaledInputs => scaledInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, BigDouble> ScaledOutputs => scaledOutputs;
        /*
         * The entities and quantities which are consumed by this entity as inputs to their production.
         */
        public Dictionary<string, BigDouble> FixedInputs => fixedInputs;
        /*
         * The entities and quantities that this entity produces each tick, and the entities and quantities that are required to produce without being consumed and entities and quantities which are consumed to produce.
         */
        public Dictionary<string, BigDouble> FixedOutputs => fixedOutputs;
        public Dictionary<string, BigDouble> Requirements => requirements;
        public BigDouble RealQuantity => _quantity;
        public BigDouble QuantityChangePerSecond { get => quantityChangePerSecond; 
            set => quantityChangePerSecond = value; }
        public BigDouble QuantityBought { get => quantityBought; set => quantityBought = value; }
        public bool IsAvailable => definition.IsAvailableMatcher.Matches(engine);
        public bool IsEnabled => definition.IsEnabledMatcher.Matches(engine);
        public bool IsVisible => definition.IsVisibleMatcher.Matches(engine);

        public Dictionary<string, Entity> Variants => variants;

        public  Entity(IEntityDefinition definition, IdleEngine engine)
        {
            validateCustomProperties(definition.CustomProperties);
            this.definition = definition;
            _quantity = definition.StartingQuantity;
            customProperties = definition.CustomProperties;
            this.engine = engine;
            quantityChangePerSecond = 0;
            foreach (var instance in definition.Variants)
            {
                variants.Add(instance.Key, new Entity(instance.Value, engine));
            }

            foreach (var customProperty in engine.Configuration.CommonEntityStringProperties)
            {
                if (!definition.CustomProperties.ContainsProperty(customProperty.Key))
                {
                    engine.Log(String.Format("Adding universtal property {1} to entity {0} with initial value of {2}", EntityKey, customProperty.Key, customProperty.Value), Logger.Level.DEBUG);
                    customProperties.Set(customProperty.Key, customProperty.Value);
                }
            }
            foreach (var customProperty in engine.Configuration.CommonEntityBooleanProperties)
            {
                if (!definition.CustomProperties.ContainsProperty(customProperty.Key))
                {
                    engine.Log(String.Format("Adding universtal property {1} to entity {0} with initial value of {2}", EntityKey, customProperty.Key, customProperty.Value), Logger.Level.DEBUG);
                    customProperties.Set(customProperty.Key, customProperty.Value);
                }
            }
            foreach (var customProperty in engine.Configuration.CommonEntityNumberProperties)
            {
                if (!definition.CustomProperties.ContainsProperty(customProperty.Key))
                {
                    engine.Log(String.Format("Adding universtal property {1} to entity {0} with initial value of {2}", EntityKey, customProperty.Key, customProperty.Value), Logger.Level.DEBUG);
                    customProperties.Set(customProperty.Key, customProperty.Value);
                }
            }
            foreach(var modifierDefinition in definition.Modifiers)
            {
                modifiers.Add(new Modifier(modifierDefinition));
            }
        }

        private void validateCustomProperties(PropertyHolder properties)
        {
            foreach (var property in properties.BooleanProperties.Keys)
            {
                if (PREDEFINED_PROPERTIES.ContainsKey(property))
                {
                    throw new ReservedEntityPropertyNameException(property);
                }
            }
            foreach (var property in properties.NumberProperties.Keys)
            {
                if (PREDEFINED_PROPERTIES.ContainsKey(property))
                {
                    throw new ReservedEntityPropertyNameException(property);
                }
            }
            foreach (var property in properties.StringProperties.Keys)
            {
                if (PREDEFINED_PROPERTIES.ContainsKey(property))
                {
                    throw new ReservedEntityPropertyNameException(property);
                }
            }
            foreach (var property in properties.ListProperties.Keys)
            {
                if (PREDEFINED_PROPERTIES.ContainsKey(property))
                {
                    throw new ReservedEntityPropertyNameException(property);
                }
            }
            foreach (var property in properties.MapProperties.Keys)
            {
                if (PREDEFINED_PROPERTIES.ContainsKey(property))
                {
                    throw new ReservedEntityPropertyNameException(property);
                }
            }
        }
        public void Buy()
        {
            Buy(1, true);
        }
        public void Buy(BigDouble quantityToBuy, bool buyAllOrNone)
        {
            engine.BuyEntity(this, quantityToBuy, buyAllOrNone);
        }
        public void ChangeQuantity(BigDouble changeBy)
        {
            if (definition.CalculatedQuantity == null)
            {
                _quantity += changeBy;
                _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
                if (_quantity > highestQuantityAchieved)
                {
                    highestQuantityAchieved = _quantity;
                }
            }
        }

        public void SetQuantity(BigDouble newQuantity)
        {
            if (definition.CalculatedQuantity == null)
            {
                _quantity = newQuantity;
                _quantity = BigDouble.Min(_quantity, definition.QuantityCap.Get(engine));
                if (_quantity > highestQuantityAchieved)
                {
                    highestQuantityAchieved = _quantity;
                }
            }
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


        public override string ToString()
        {
            return String.Format("GameEntity({0}) x {1} + {2}/sec", this.EntityKey, this.Quantity, quantityChangePerSecond);
        }


        public void Update(IdleEngine engine, float deltaTime)
        {
            recalculateProperties();
            foreach(var modifier in modifiers)
            {
                if(IsEnabled)
                {
                    modifier.Update(engine, deltaTime);
                }
            }
        }

        private void recalculateProperties()
        {
            var parent = engine.GetEntity(EntityKey);
            foreach (var key in engine.AllEntities.Keys)
            {
                costs[key] = BaseCosts.ContainsKey(key) ? BaseCosts[key].Get(engine) : parent.BaseCosts.ContainsKey(key) ? parent.BaseCosts[key].Get(engine) : 0;
                fixedInputs[key] = BaseFixedInputs.ContainsKey(key) ? BaseFixedInputs[key].Get(engine) : parent.BaseFixedInputs.ContainsKey(key) ? parent.BaseFixedInputs[key].Get(engine) : 0;
                fixedOutputs[key] = BaseFixedOutputs.ContainsKey(key) ? BaseFixedOutputs[key].Get(engine) : parent.BaseFixedOutputs.ContainsKey(key) ? parent.BaseFixedOutputs[key].Get(engine) : 0;
                scaledInputs[key] = BaseScaledInputs.ContainsKey(key) ? BaseScaledInputs[key].Get(engine) : parent.BaseScaledInputs.ContainsKey(key) ? parent.BaseScaledInputs[key].Get(engine) : 0;
                scaledOutputs[key] = BaseScaledOutputs.ContainsKey(key) ? BaseScaledOutputs[key].Get(engine) : parent.BaseScaledOutputs.ContainsKey(key) ? parent.BaseScaledOutputs[key].Get(engine) : 0;
                upkeep[key] = BaseUpkeep.ContainsKey(key) ? BaseUpkeep[key].Get(engine) : parent.BaseUpkeep.ContainsKey(key) ? parent.BaseUpkeep[key].Get(engine) : 0;
                requirements[key] = BaseRequirements.ContainsKey(key) ? BaseRequirements[key].Get(engine) : parent.BaseRequirements.ContainsKey(key) ? parent.BaseRequirements[key].Get(engine) : 0;
            }
            foreach(var variant in variants.Values)
            {
                variant.recalculateProperties();
            }
            if(!Accumulates)
            {
                _quantity = 0;
            }
            if(definition.CalculatedQuantity != null)
            {
                _quantity = definition.CalculatedQuantity.Get(engine);
                if (_quantity > highestQuantityAchieved)
                {
                    highestQuantityAchieved = _quantity;
                }
            }
        }

        private string getPredefinedStringProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Name":
                    return Name;
                default:
                    throw new InvalidOperationException();
            }
        }
        private BigDouble getPredefinedNumberProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Quantity":
                    return Quantity;
                case "QuantityCap":
                    return QuantityCap;
                case "QuantityChangePerSecond":
                    return QuantityChangePerSecond;
                case "QuantityBought":
                    return QuantityBought;
                case "HighestQuantityAchieved":
                    return highestQuantityAchieved;
                case "Progress":
                    return Progress;
                default:
                    throw new InvalidOperationException();
            }
        }
        private Dictionary<string, ValueContainer> getPredefinedMapProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "ScaledInputs":
                    return convertToContainer(scaledInputs);
                case "ScaledOutputs":
                    return convertToContainer(scaledOutputs);
                case "FixedInputs":
                    return convertToContainer(fixedInputs);
                case "FixedOutputs":
                    return convertToContainer(FixedOutputs);
                case "Upkeep":
                    return convertToContainer(Upkeep);
                default:
                    throw new InvalidOperationException();
            }
        }
        private bool getPredefinedBooleanProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Enabled":
                    return IsEnabled;
                case "Available":
                    return IsAvailable;
                case "Visible":
                    return IsVisible;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void assertEntityHasProperty(string entityKey, string propertyName)
        {
            if (!customProperties.ContainsProperty(propertyName))
            {
                throw new MissingEntityPropertyException(entityKey, propertyName);
            }
        }

        public string GetStringProperty(string propertyName)
        {
            if (PREDEFINED_PROPERTY_TYPES["string"].Contains(propertyName))
            {
                return getPredefinedStringProperty(propertyName);
            }
            if (!customProperties.ContainsProperty(propertyName) && EntityKey != VariantKey)
            {
                return engine.GetEntity(EntityKey).GetStringProperty(propertyName);
            }
            assertEntityHasProperty(EntityKey, propertyName);
            return customProperties.GetString(propertyName).Get(engine);
        }

        public BigDouble GetNumberProperty(string propertyName)
        {
            string[] pathTokens = propertyName.Split('.');
            if (PREDEFINED_PROPERTY_TYPES["map"].Contains(pathTokens[0]))
            {
                var mapContainer = getPredefinedMapProperty(pathTokens[0]);
                return mapContainer[pathTokens[1]].AsNumber().Get(engine);
            }
            if (PREDEFINED_PROPERTY_TYPES["number"].Contains(propertyName))
            {
                return getPredefinedNumberProperty(propertyName);
            }
            if (!customProperties.ContainsProperty(propertyName) && EntityKey != VariantKey)
            {
                return engine.GetEntity(EntityKey).GetNumberProperty(propertyName);
            }
            assertEntityHasProperty(EntityKey, propertyName);
            return customProperties.GetNumber(propertyName).Get(engine);

        }
        public bool GetBooleanProperty(string propertyName)
        {
            if (PREDEFINED_PROPERTY_TYPES["boolean"].Contains(propertyName))
            {
                return getPredefinedBooleanProperty(propertyName);
            }
            if (!customProperties.ContainsProperty(propertyName))
            {
                throw new MissingEntityPropertyException(EntityKey, propertyName);
            }
            if (!customProperties.ContainsProperty(propertyName) && EntityKey != VariantKey)
            {
                return engine.GetEntity(EntityKey).GetBooleanProperty(propertyName);
            }
            assertEntityHasProperty(EntityKey, propertyName);
            return customProperties.GetBoolean(propertyName).Get(engine);
        }
        public Dictionary<string, ValueContainer> GetMapProperty(string propertyName)
        {
            if (PREDEFINED_PROPERTY_TYPES["map"].Contains(propertyName))
            {
                return getPredefinedMapProperty(propertyName);
            }
            if (!customProperties.ContainsProperty(propertyName) && EntityKey != VariantKey)
            {
                return engine.GetEntity(EntityKey).GetMapProperty(propertyName);
            }
            assertEntityHasProperty(EntityKey, propertyName);
            var propertyMap = customProperties.GetMap(propertyName);
            return propertyMap.Get(engine);
        }

        private static Dictionary<string, ValueContainer> convertToContainer(Dictionary<string, BigDouble> properties)
        {
            var containers = new Dictionary<string, ValueContainer>();
            foreach (var property in properties)
            {
                containers.Add(property.Key, Literal.Of(property.Value));
            }
            return containers;
        }

        public Entity GetVariant(string instanceKey)
        {
            if (!variants.ContainsKey(instanceKey))
            {
                throw new MissingEntityInstanceException(EntityKey, instanceKey);
            }
            return variants[instanceKey];
        }

        public EntitySnapshot GetSnapshot(IdleEngine engine)
        {
            return new EntitySnapshot(EntityKey, _quantity, quantityBought, highestQuantityAchieved);
        }

        public void LoadFromSnapshot(EntitySnapshot snapshot)
        {
            if(snapshot.EntityKey != EntityKey)
            {
                throw new InvalidOperationException("Entity keys don't match!");
            }
            _quantity = snapshot.Quantity;
            quantityBought = snapshot.QuantityBought;
            highestQuantityAchieved = snapshot.HighestQuantityAchieved;
        }

        public BigDouble HighestEntityQuantity => highestQuantityAchieved;
    }

}