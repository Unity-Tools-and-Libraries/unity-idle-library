using BreakInfinity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class IdleEngine
    {
        private static readonly Logger logger = Logger.GetLogger();
        private float accruedTime;
        private readonly GameConfiguration configuration;
        private readonly Dictionary<string, GameEntity> _allEntities = new Dictionary<string, GameEntity>();
        private readonly ISet<Modifier> _modifiers = new HashSet<Modifier>();
        private readonly Dictionary<string, GameEntity> _resources = new Dictionary<string, GameEntity>();
        private readonly HookManager hooks;

        private readonly Dictionary<string, SingletonEntityDefinition> singletons = new Dictionary<string, SingletonEntityDefinition>();
        private readonly Dictionary<string, ModifiableProperty> globalProperties = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, string> globalSingletonProperties = new Dictionary<string, string>();
        private readonly Dictionary<string, Tutorial> tutorials = new Dictionary<string, Tutorial>();
        private readonly Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();


        private bool startHookFired = false;

        private System.Timers.Timer updateThrottleTimer = new System.Timers.Timer(100);

        public void SetLogLevel(Logger.Level level)
        {
            Logger.globalLevel = level;
        }

        private ISet<Modifier> lastActiveModifiers = new HashSet<Modifier>();

        public Dictionary<string, GameEntity> AllEntities => _allEntities;

        public ISet<Modifier> Modifiers => _modifiers;

        public Dictionary<string, SingletonEntityDefinition> AllSingletons => singletons;
        public Dictionary<string, Tutorial> Tutorials => tutorials;
        public IdleEngine(GameConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration must not be null");
            }
            this.configuration = configuration;
            foreach (SingletonEntityDefinition singleton in configuration.Singletons)
            {
                singletons.Add(singleton.SingletonTypeKey, singleton);
            }
            foreach (var globalProperty in configuration.GlobalProperties)
            {
                this.globalProperties.Add(globalProperty.Key, new ModifiableProperty(null, globalProperty.Key, globalProperty.Value, this));
            }
            foreach (TutorialConfiguration tutorial in configuration.Tutorials)
            {
                tutorials.Add(tutorial.TutorialKey, new Tutorial(tutorial));
            }
            hooks = new HookManager(configuration.Hooks, this);
            setupAchievements();
            foreach (var globalSingleton in configuration.GlobalSingletonProperties)
            {
                globalSingletonProperties[globalSingleton.Key] = globalSingleton.Value.DefaultInstance;
            }
            setupEntities();
            setupModifiers();
        }

        public void DispatchEvent(string eventName)
        {
            DispatchEvent(eventName, null);
        }

        public void DispatchEvent(string eventName, object arg)
        {
            hooks.ExecuteEventHook(eventName, arg);
        }

        private void setupAchievements()
        {
            foreach (var achievementConfig in configuration.Achievements)
            {
                var achievement = new Achievement(achievementConfig.Value);
                achievements.Add(achievementConfig.Key, achievement);
            }
        }

        private void setupModifiers()
        {
            foreach (ModifierDefinition modifierDefinition in configuration.Modifiers)
            {
                var modifierEffects = new HashSet<Effect>();
                foreach (var effectDefinition in modifierDefinition.Effects)
                {
                    var effect = new Effect(effectDefinition, this);
                    modifierEffects.Add(effect);
                }
                var modifier = new Modifier(modifierDefinition, modifierEffects);
                foreach (var effect in modifierEffects)
                {
                    var affectable = effect.Definition.GetAffectableProperties(this);
                    foreach (var canBeAffected in affectable)
                    {
                        logger.Debug(String.Format("Adding {0} from modifier {1} to {2}", effect, modifier.ModifierKey, canBeAffected));
                        canBeAffected.AddModifierEffect(new ModifierEffect(modifier, effect));
                    }
                }
                _modifiers.Add(modifier);
            }
        }

        private void setupEntities()
        {
            foreach (EntityDefinition entity in configuration.Entities)
            {
                foreach (var customProperty in configuration.SharedEntityProperties)
                {
                    if (!entity.CustomProperties.ContainsKey(customProperty.Key))
                    {
                        logger.Debug(String.Format("Adding universtal property {1} to entity {0} with initial value of {2}", entity.EntityKey, customProperty.Key, customProperty.Value));
                        entity.CustomProperties.Add(customProperty.Key, customProperty.Value);
                    }
                }
                GameEntity entityInstance = new GameEntity(entity, this);
                foreach (var otherEntity in configuration.Entities)
                {
                    ValueContainer baseInput;
                    ValueContainer baseOutput;
                    ValueContainer baseFixedInput;
                    ValueContainer baseFixedOutput;
                    ValueContainer baseUpkeep;
                    ValueContainer baseCost;
                    ValueContainer baseRequirement;
                    if (!entityInstance.BaseProductionInputs.TryGetValue(otherEntity.EntityKey, out baseInput))
                    {
                        baseInput = Literal.Of(0);
                    }
                    if (!entityInstance.BaseProductionOutputs.TryGetValue(otherEntity.EntityKey, out baseOutput))
                    {
                        baseOutput = Literal.Of(0);
                    }
                    if (!entityInstance.BaseUpkeep.TryGetValue(otherEntity.EntityKey, out baseUpkeep))
                    {
                        baseUpkeep = Literal.Of(0);
                    }
                    if (!entityInstance.BaseCosts.TryGetValue(otherEntity.EntityKey, out baseCost))
                    {
                        baseCost = Literal.Of(0);
                    }
                    if (!entityInstance.BaseRequirements.TryGetValue(otherEntity.EntityKey, out baseRequirement))
                    {
                        baseRequirement = Literal.Of(0);
                    }
                    if (!entityInstance.BaseFixedProductionInputs.TryGetValue(otherEntity.EntityKey, out baseFixedInput))
                    {
                        baseFixedInput = Literal.Of(0);
                    }

                    if (!entityInstance.BaseFixedProductionOutputs.TryGetValue(otherEntity.EntityKey, out baseFixedOutput))
                    {
                        baseFixedOutput = Literal.Of(0);
                    }
                    entityInstance.Inputs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "inputs-" + otherEntity.EntityKey, baseInput, this);
                    entityInstance.Outputs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "outputs-" + otherEntity.EntityKey, baseOutput, this);
                    entityInstance.FixedInputs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "fixedInputs-" + otherEntity.EntityKey, baseFixedInput, this);
                    entityInstance.FixedOutputs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "fixedOutputs-" + otherEntity.EntityKey, baseFixedOutput, this);
                    entityInstance.Upkeep[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "upkeep-" + otherEntity.EntityKey, baseUpkeep, this);
                    entityInstance.Costs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "costs-" + otherEntity.EntityKey, baseCost, this);
                }

                _allEntities.Add(entity.EntityKey, entityInstance);
            }
            foreach (EntityDefinition entity in configuration.Entities)
            {
                if (entity.CalculatedQuantity != null)
                {
                    BigDouble calculatedStartingQuantity = entity.CalculatedQuantity.GetAsNumber(this);
                    AllEntities[entity.EntityKey].SetQuantity(calculatedStartingQuantity);
                }
            }
        }

        public SingletonEntityInstance GetGlobalSingleton(string property)
        {
            string instanceKey;
            globalSingletonProperties.TryGetValue(property, out instanceKey);
            if (instanceKey == null)
            {
                return null;
            }
            return singletons[configuration.GlobalSingletonProperties[property].Type].Instances[instanceKey];
        }

        public BigDouble GetProductionForEntity(string entityKey)
        {
            return AllEntities[entityKey].QuantityChangePerSecond.GetAsNumber(this);
        }

        public Achievement GetAchievement(string achievementKey)
        {
            Achievement achievement;
            achievements.TryGetValue(achievementKey, out achievement);
            return achievement;
        }

        public object GetRawGlobalProperty(string propertyName)
        {
            string[] tokenized = propertyName.Split('.');
            object value = globalProperties.ContainsKey(tokenized[0]) ? globalProperties[tokenized[0]] : null;
            for (int i = 1; i < tokenized.Length; i++)
            {
                if (value == null)
                {
                    return null;
                }
                string nextProperty = tokenized[i];
                var propertyReference = value.GetType().GetProperty(nextProperty);
                if (propertyReference != null)
                {
                    value = propertyReference.GetValue(value);
                }
            }
            return value;
        }

        public BigDouble GetGlobalNumberProperty(string propertyName)
        {
            ModifiableProperty property;
            globalProperties.TryGetValue(propertyName, out property);
            return property != null ? property.GetAsNumber(this) : 0;
        }

        public bool GetGlobalBooleanProperty(string propertyName)
        {
            ModifiableProperty property;
            globalProperties.TryGetValue(propertyName, out property);
            return property != null ? property.GetAsBoolean(this) : false;
        }

        public string GetGlobalStringProperty(string propertyName)
        {
            ModifiableProperty property;
            globalProperties.TryGetValue(propertyName, out property);
            return property != null ? GetLocalizedString(property.GetAsString(this)) : "";
        }

        public void BuyEntity(GameEntity gameEntity, BigDouble quantityToBuy, bool buyAllOrNothing)
        {
            if (!gameEntity.IsAvailable)
            {
                return;
            }
            foreach (var resource in gameEntity.Costs)
            {
                var maxPurchaseable = resource.Value.GetAsNumber(this) == 0 ? quantityToBuy : BigDouble.Floor(_allEntities[resource.Key].Quantity / resource.Value.GetAsNumber(this) * quantityToBuy);
                if (buyAllOrNothing && maxPurchaseable < quantityToBuy)
                {
                    quantityToBuy = 0;
                }
                else
                {
                    quantityToBuy = BigDouble.Min(maxPurchaseable, quantityToBuy);
                }
                if (quantityToBuy == 0)
                {
                    break;
                }
            }
            if (quantityToBuy > 0)
            {
                UpdateResourcesFromEntityPurchase(gameEntity, quantityToBuy);
            }
        }

        private void RecalculateAllEntityProperties(float deltaTime)
        {
            foreach (var entity in AllEntities.Values)
            {
                if (entity.IsEnabled)
                {
                    logger.Debug(String.Format("Updating properties of active entity {0}", entity.EntityKey));
                    entity.ForEachProperty((name, property) =>
                    {
                        property.Update(this, deltaTime);
                        logger.Debug(string.Format("New value {0}", property.RawValue(this)));
                    });
                }
                else
                {
                    logger.Debug(String.Format("Skipping inactive entity {0}", entity.EntityKey));
                }
            }
        }

        public BigDouble PerformUpkeepForEntity(GameEntity entity, float deltaTime)
        {
            if (entity.ShouldBeDisabled(this))
            {
                return 0;
            }
            BigDouble quantity = entity.RealQuantity;
            // Determine max number can be supported.
            foreach (var resource in entity.Upkeep)
            {
                var maxQuantityPossible = resource.Value.GetAsNumber(this) > 0 ? BigDouble.Floor(_allEntities[resource.Key].Quantity / (resource.Value.GetAsNumber(this) * deltaTime)) : quantity;
                quantity = BigDouble.Min(quantity, maxQuantityPossible);
            }
            // Consume upkeep requirements
            foreach (var resource in entity.Upkeep)
            {
                _allEntities[resource.Key].ChangeQuantity(-resource.Value.GetAsNumber(this) * BigDouble.Floor(quantity) * deltaTime);
            }
            // Set new quantity
            entity.SetQuantity(quantity);
            return quantity;
        }

        private void assertEntityExists(string key)
        {
            if (!_allEntities.ContainsKey(key))
            {
                throw new InvalidProgramException(String.Format("Referenced entity {0} doesn't exist", key));
            }
        }

        public bool SetGlobalSingletonProperty(string propertyName, string singletonInstanceKey)
        {
            var singletonTypeKey = this.configuration.GlobalSingletonProperties;
            return false;
        }

        internal void UpdateResourcesFromEntityPurchase(GameEntity gameEntity, BigDouble quantityToBuy)
        {
            foreach (var cost in gameEntity.Costs)
            {
                _allEntities[cost.Key].ChangeQuantity(-cost.Value.GetAsNumber(this) * quantityToBuy);
            }
            gameEntity.ChangeQuantity(quantityToBuy);
        }

        /**
         *Change the quantity of the resource with the given key, by the given amount.
         * Returns the quantity of the change or 0 if the entity doesn't exist.
         */
        public BigDouble ChangeEntityQuantity(string entityKey, BigDouble quantityToAdd)
        {
            GameEntity entity = null;
            if (_allEntities.TryGetValue(entityKey, out entity))
            {
                entity.ChangeQuantity(quantityToAdd);
                return quantityToAdd;
            }
            else
            {
                Debug.Log(string.Format("Tried to change quantity of entity {0}, which wasn't found", entityKey));
                return 0;
            }
        }

        /**
         * Change the quantity of the resource with the given key, by the given amount.
         * Returns the new quantity or 0 if the entity doesn't exist.
         */
        public BigDouble SetEntityQuantity(string entityKey, BigDouble quantityToSet)
        {
            GameEntity entity = null;
            if (_allEntities.TryGetValue(entityKey, out entity))
            {
                entity.SetQuantity(quantityToSet);
                return quantityToSet;
            }
            else
            {
                Debug.Log(string.Format("Tried to change quantity of entity {0}, which wasn't found", entityKey));
                return 0;
            }
        }

        /**
         * Advance the state of the framework by one tick.
         */
        public void Update(float deltaTime)
        {
            accruedTime += deltaTime;
            if (accruedTime > .1)
            {
                doUpdate(accruedTime);
                accruedTime = 0;
            }
        }

        private void doUpdate(float deltaTime)
        {
            if (!startHookFired)
            {
                hooks.ExecuteEngineStartHooks();
                startHookFired = true;
            }
            RecalculateAllEntityProperties(deltaTime);
            Dictionary<string, BigDouble> productionResult = DoEntityProduction(deltaTime);
            Dictionary<string, BigDouble> upkeepResult = DoEntityUpkeep(deltaTime, productionResult);
            foreach (var entity in AllEntities)
            {
                entity.Value.QuantityChangePerSecond.SetBaseValue(Literal.Of(productionResult[entity.Key] - upkeepResult[entity.Key]));
                entity.Value.Update(this, 1, deltaTime);
            }
            foreach (Achievement achievement in achievements.Values)
            {
                if (achievement.ShouldBeActive(this))
                {
                    achievement.Gain();
                }
            }
            foreach (var property in globalProperties.Values)
            {
                property.Update(this, deltaTime);
            }
            foreach (var tutorial in tutorials.Values)
            {
                tutorial.Update(this, deltaTime);
            }
            hooks.ExecuteUpdateHooks(deltaTime);
        }

        private Dictionary<string, BigDouble> DoEntityUpkeep(float deltaTime, Dictionary<string, BigDouble> productionResult)
        {
            Dictionary<string, BigDouble> quantitiesConsumed = new Dictionary<string, BigDouble>();
            foreach (var entity in AllEntities)
            {
                var quantitySupported = entity.Value.Quantity;
                foreach (var upkeep in entity.Value.Upkeep)
                {
                    var maxSupportable = (productionResult[upkeep.Key] + AllEntities[upkeep.Key].Quantity) / upkeep.Value.GetAsNumber(this);
                    if (BigDouble.IsNaN(maxSupportable) || BigDouble.IsInfinity(maxSupportable))
                    {
                        maxSupportable = quantitySupported;
                    }
                    quantitySupported = BigDouble.Min(maxSupportable, quantitySupported);
                }
                foreach (var upkeep in entity.Value.Upkeep)
                {
                    BigDouble quantityToConsume = BigDouble.Floor(quantitySupported) * upkeep.Value.GetAsNumber(this);
                    BigDouble previousValue;
                    quantitiesConsumed.TryGetValue(upkeep.Key, out previousValue);
                    quantitiesConsumed[upkeep.Key] = previousValue + quantityToConsume;
                }
                entity.Value.SetQuantity(quantitySupported);
            }
            return quantitiesConsumed;
        }

        private Dictionary<string, BigDouble> DoEntityProduction(float deltaTime)
        {
            return calculateProductionOutput();
        }

        private Dictionary<string, BigDouble> calculateProductionOutput()
        {
            Dictionary<string, BigDouble> outputs = new Dictionary<string, BigDouble>();
            foreach (var entityProducing in AllEntities.Values)
            {
                if (!entityProducing.IsEnabled)
                {
                    continue;
                }
                var entityOutputs = new Dictionary<string, BigDouble>();
                var quantityAbleToProduce = entityProducing.Quantity;
                foreach (var input in entityProducing.Inputs)
                {
                    if (quantityAbleToProduce == BigDouble.Zero)
                    {
                        break;
                    }
                    BigDouble maxSupportable = AllEntities[input.Key].Quantity / entityProducing.Inputs[input.Key].GetAsNumber(this);
                    if (BigDouble.IsNaN(maxSupportable) || BigDouble.IsInfinity(maxSupportable))
                    {
                        maxSupportable = quantityAbleToProduce;
                    }
                    quantityAbleToProduce = BigDouble.Min(maxSupportable, quantityAbleToProduce);
                }
                quantityAbleToProduce = BigDouble.Floor(quantityAbleToProduce);
                foreach (var output in entityProducing.FixedOutputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(output.Key, out currentValue);
                    var outputValue = output.Value.GetAsNumber(this);
                    outputs[output.Key] = currentValue + (outputValue);
                }
                foreach (var output in entityProducing.Outputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(output.Key, out currentValue);
                    var outputValue = output.Value.GetAsNumber(this);
                    outputs[output.Key] = currentValue + (outputValue * quantityAbleToProduce);
                }
                foreach (var input in entityProducing.FixedInputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(input.Key, out currentValue);
                    var inputValue = input.Value.GetAsNumber(this);

                    outputs[input.Key] = currentValue - (inputValue);
                }
                foreach (var input in entityProducing.Inputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(input.Key, out currentValue);
                    var inputValue = input.Value.GetAsNumber(this);

                    outputs[input.Key] = currentValue - (inputValue * quantityAbleToProduce);
                }
            }
            return outputs;
        }

        public string GetLocalizedString(string key)
        {
            string value = key;
            if (configuration.Strings.TryGetValue(key, out value))
            {
                return value;
            }
            return key;
        }

        public object Traverse(object subject, string properties)
        {
            return Traverse(subject, properties.Split('.'));
        }

        public object Traverse(object subject, string[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (subject == null)
                {
                    break;
                }
                if (typeof(Dictionary<string, ModifiableProperty>) == subject.GetType())
                {
                    var dictionary = (Dictionary<string, ModifiableProperty>)subject;
                    if (dictionary.ContainsKey(properties[i]))
                    {
                        subject = dictionary[properties[i]];
                    }
                    else
                    {
                        subject = null;
                    }
                }
                else
                {
                    var propertyReference = subject.GetType().GetProperty(properties[i]);
                    if (propertyReference == null)
                    {
                        subject = null;
                    }
                    else
                    {
                        subject = propertyReference.GetValue(subject);
                    }
                }
            }
            if (subject == null)
            {
                return null;
            }
            return subject;
        }
    }
}