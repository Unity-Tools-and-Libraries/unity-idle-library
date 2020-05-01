using BreakInfinity;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using IdleFramework.Events;
using System.Linq;
using IdleFramework.UI.Components.Generators;
using IdleFramework.UI.Components;

namespace IdleFramework
{
    public class IdleEngine : CanSnapshot<IdleEngineSnapshot>
    {
        private static readonly Logger logger = Logger.GetLogger();
        private float accruedTime;
        private DateTime lastUpdateTime;
        private readonly EventManager events;
        private readonly GameConfiguration configuration;
        private readonly Dictionary<string, Entity> _allEntities = new Dictionary<string, Entity>();
        private readonly Dictionary<string, List<Entity>> entitiesByType = new Dictionary<string, List<Entity>>();
        private readonly Dictionary<string, Entity> _resources = new Dictionary<string, Entity>();
        private readonly IOfflinePolicy offlinePolicy;
        private readonly UiGenerator uiGenerator = new UiGenerator();

        private readonly HookManager hooks;

        private readonly PropertyHolder globalProperties = new PropertyHolder();

        private readonly Dictionary<string, Tutorial> tutorials = new Dictionary<string, Tutorial>();
        private readonly Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();


        private bool startHookFired = false;

        private System.Timers.Timer updateThrottleTimer = new System.Timers.Timer(100);

        public void SetLogLevel(Logger.Level level)
        {
            Logger.globalLevel = level;
        }

        public Dictionary<string, Entity> AllEntities => _allEntities;
        public Dictionary<string, Tutorial> Tutorials => tutorials;
        internal GameConfiguration Configuration => configuration;
        public EventManager Events => events;

        public IdleEngine(GameConfiguration configuration, GameObject gameObject)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration must not be null");
            }
            events = new EventManager(gameObject);
            this.configuration = configuration;
            foreach (var globalProperty in configuration.GlobalStringProperties)
            {
                this.globalProperties.Set(globalProperty.Key, globalProperty.Value);
            }
            foreach (var globalProperty in configuration.GlobalNumberProperties)
            {
                this.globalProperties.Set(globalProperty.Key, globalProperty.Value);
            }
            foreach (var globalProperty in configuration.GlobalBooleanProperties)
            {
                this.globalProperties.Set(globalProperty.Key, globalProperty.Value);
            }
            foreach (TutorialConfiguration tutorial in configuration.Tutorials)
            {
                tutorials.Add(tutorial.TutorialKey, new Tutorial(tutorial));
            }
            hooks = new HookManager(configuration.Hooks, this);
            offlinePolicy = configuration.OfflinePolicy;
            setupAchievements();
            foreach (EntityDefinition entityDefinition in configuration.Entities.Values)
            {
                Entity instance = new Entity(entityDefinition, this);

                _allEntities.Add(entityDefinition.EntityKey, instance);
                foreach (var type in entityDefinition.Types)
                {
                    List<Entity> entities;
                    if (!entitiesByType.TryGetValue(type, out entities))
                    {
                        entities = new List<Entity>();
                        entitiesByType.Add(type, entities);
                    }
                    entities.Add(instance);
                }
            }
            setupEntities();
            if (configuration.UiConfiguration != null)
            {
                setupUi(gameObject);
            }
        }

        public void SetGlobalProperty(string propertyName, StringContainer value)
        {
            globalProperties.Set(propertyName, value);
        }
        public void SetGlobalProperty(string propertyName, string value)
        {
            globalProperties.Set(propertyName, Literal.Of(value));
        }

        public void SetGlobalProperty(string propertyName, NumberContainer value)
        {
            globalProperties.Set(propertyName, value);
        }
        public void SetGlobalProperty(string propertyName, BooleanContainer value)
        {
            globalProperties.Set(propertyName, value);
        }
        public void SetGlobalProperty(string propertyName, MapContainer value)
        {
            globalProperties.Set(propertyName, value);
        }
        private void setupAchievements()
        {
            foreach (var achievementConfig in configuration.Achievements)
            {
                var achievement = new Achievement(achievementConfig.Value);
                achievements.Add(achievementConfig.Key, achievement);
            }
        }

        private void setupEntities()
        {
            foreach (Entity entity in AllEntities.Values)
            {
                setupEntity(entity);
            }
            foreach (EntityDefinition entity in configuration.Entities.Values)
            {
                if (entity.CalculatedQuantity != null)
                {
                    BigDouble calculatedStartingQuantity = entity.CalculatedQuantity.Get(this);
                    AllEntities[entity.EntityKey].SetQuantity(calculatedStartingQuantity);
                }
            }
        }

        private void setupEntity(Entity entityInstance)
        {
            var entityConfiguration = configuration.Entities[entityInstance.EntityKey];
            foreach (var variant in entityInstance.Variants.Values)
            {
                setupEntity(variant);
            }
        }

        public MapContainer GetGlobalMapProperty(string property)
        {
            if (!globalProperties.ContainsProperty(property))
            {
                throw new MissingGlobalPropertyException(property);
            }
            return globalProperties.GetMap(property);
        }

        public NumberContainer GetGlobalNumberProperty(string property)
        {
            if (!globalProperties.ContainsProperty(property))
            {
                throw new MissingGlobalPropertyException(property);
            }
            return globalProperties.GetNumber(property);
        }

        public StringContainer GetGlobalStringProperty(string property)
        {
            if (!globalProperties.ContainsProperty(property))
            {
                throw new MissingGlobalPropertyException(property);
            }
            return globalProperties.GetString(property);
        }

        public BooleanContainer GetGlobalBooleanProperty(string property)
        {
            if (!globalProperties.ContainsProperty(property))
            {
                throw new MissingGlobalPropertyException(property);
            }
            return globalProperties.GetBoolean(property);
        }

        public BigDouble GetProductionForEntity(string entityKey)
        {
            return AllEntities[entityKey].QuantityChangePerSecond;
        }

        public Achievement GetAchievement(string achievementKey)
        {
            Achievement achievement;
            achievements.TryGetValue(achievementKey, out achievement);
            return achievement;
        }
        public void BuyEntity(Entity gameEntity)
        {
            BuyEntity(gameEntity, 1, true);
        }
        public void BuyEntity(Entity gameEntity, BigDouble quantityToBuy, bool buyAllOrNothing)
        {
            if (!gameEntity.IsAvailable)
            {
                return;
            }
            hooks.ExecuteBeforeBuyHooks(gameEntity);
            foreach (var resource in gameEntity.Costs)
            {
                var maxPurchaseable = resource.Value == 0 ? quantityToBuy : BigDouble.Floor(_allEntities[resource.Key].Quantity / resource.Value * quantityToBuy);
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
                gameEntity.QuantityBought += quantityToBuy;
                UpdateResourcesFromEntityPurchase(gameEntity, quantityToBuy);
            }
        }
        public BigDouble PerformUpkeepForEntity(Entity entity, float deltaTime)
        {
            if (!entity.IsEnabled)
            {
                return 0;
            }
            BigDouble quantity = entity.RealQuantity;
            // Determine max number can be supported.
            foreach (var resource in entity.Upkeep)
            {
                var maxQuantityPossible = resource.Value > 0 ? BigDouble.Floor(_allEntities[resource.Key].Quantity / (resource.Value * deltaTime)) : quantity;
                quantity = BigDouble.Min(quantity, maxQuantityPossible);
            }
            // Consume upkeep requirements
            foreach (var resource in entity.Upkeep)
            {
                _allEntities[resource.Key].ChangeQuantity(-resource.Value * BigDouble.Floor(quantity) * deltaTime);
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

        internal void UpdateResourcesFromEntityPurchase(Entity gameEntity, BigDouble quantityToBuy)
        {
            foreach (var cost in gameEntity.Costs)
            {
                _allEntities[cost.Key].ChangeQuantity(-cost.Value * quantityToBuy);
            }
            gameEntity.ChangeQuantity(quantityToBuy);
        }

        /**
         *Change the quantity of the resource with the given key, by the given amount.
         * Returns the quantity of the change or 0 if the entity doesn't exist.
         */
        public BigDouble ChangeEntityQuantity(string entityKey, BigDouble quantityToAdd)
        {
            Entity entity = null;
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
            Entity entity = null;
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
            lastUpdateTime = System.DateTime.UtcNow;
            accruedTime += deltaTime;
            if(accruedTime >= .1f)
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
            hooks.ExecuteUpdateHooks(deltaTime);
            foreach (var entity in AllEntities)
            {
                entity.Value.Update(this, deltaTime);
            }
            Dictionary<string, BigDouble> productionResult = DoEntityProduction(deltaTime);
            Dictionary<string, BigDouble> upkeepResult = DoEntityUpkeep(deltaTime, productionResult);
            foreach (var entity in AllEntities.Values)
            {
                var amountProduced = productionResult.ContainsKey(entity.EntityKey) ? productionResult[entity.EntityKey] : 0;
                var upkeepAmount = upkeepResult.ContainsKey(entity.EntityKey) ? upkeepResult[entity.EntityKey] : 0;
                entity.QuantityChangePerSecond = amountProduced - upkeepAmount;
                entity.ChangeQuantity(entity.QuantityChangePerSecond);
            }
            foreach (Achievement achievement in achievements.Values)
            {
                if (achievement.ShouldBeActive(this))
                {
                    achievement.Gain();
                }
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
                    BigDouble producedQuantity = 0;
                    productionResult.TryGetValue(upkeep.Key, out producedQuantity);
                    var maxSupportable = (producedQuantity + GetEntity(upkeep.Key).Quantity) / upkeep.Value;
                    if (BigDouble.IsNaN(maxSupportable) || BigDouble.IsInfinity(maxSupportable))
                    {
                        maxSupportable = quantitySupported;
                    }
                    quantitySupported = BigDouble.Min(maxSupportable, BigDouble.Ceiling(quantitySupported));
                }
                foreach (var upkeep in entity.Value.Upkeep)
                {
                    BigDouble quantityToConsume = BigDouble.Floor(quantitySupported) * upkeep.Value;
                    BigDouble previousValue;
                    quantitiesConsumed.TryGetValue(upkeep.Key, out previousValue);
                    quantitiesConsumed[upkeep.Key] = previousValue + quantityToConsume;
                }
                entity.Value.SetQuantity(BigDouble.Min(quantitySupported, entity.Value.Quantity));
            }
            return quantitiesConsumed;
        }

        private Dictionary<string, BigDouble> DoEntityProduction(float deltaTime)
        {
            return calculateProductionOutput(deltaTime);
        }

        private Dictionary<string, BigDouble> calculateProductionOutput(float deltaTime)
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
                foreach (var input in entityProducing.ScaledInputs)
                {
                    if (quantityAbleToProduce == BigDouble.Zero)
                    {
                        break;
                    }
                    BigDouble maxSupportable = AllEntities[input.Key].Quantity / entityProducing.ScaledInputs[input.Key];
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
                    var outputValue = output.Value * deltaTime;
                    outputs[output.Key] = currentValue + (outputValue);
                }
                foreach (var output in entityProducing.ScaledOutputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(output.Key, out currentValue);
                    var outputValue = output.Value * deltaTime;
                    outputs[output.Key] = currentValue + (outputValue * quantityAbleToProduce);
                }
                foreach (var input in entityProducing.FixedInputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(input.Key, out currentValue);
                    var inputValue = input.Value * deltaTime;

                    outputs[input.Key] = currentValue - (inputValue);
                }
                foreach (var input in entityProducing.ScaledInputs)
                {
                    BigDouble currentValue;
                    outputs.TryGetValue(input.Key, out currentValue);
                    var inputValue = input.Value * deltaTime;

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
        public void Log(string message, Logger.Level level)
        {
            logger.Log(level, message);
        }
        private static Regex accessMatcher = new Regex(".+[(/.+)]");
        private void assertGlobalPropertyExists(string propertyName)
        {
            if (!globalProperties.ContainsProperty(propertyName))
            {
                throw new InvalidOperationException(string.Format("Global property {0} does no exist", propertyName));
            }
        }

        public Entity GetEntity(string entityKey)
        {
            if (!AllEntities.ContainsKey(entityKey))
            {
                throw new MissingEntityException(entityKey);
            }
            return AllEntities[entityKey];
        }

        public List<Entity> GetEntitiesWithType(string type)
        {
            List<Entity> entities;
            if (!entitiesByType.TryGetValue(type, out entities))
            {
                entities = new List<Entity>();
            }
            return entities;
        }

        public IdleEngineSnapshot GetSnapshot(IdleEngine engine)
        {
            return new IdleEngineSnapshot(
                AllEntities.Values.Select(e => e.GetSnapshot(engine)).ToList(),
                achievements.Values.Select(a => a.GetSnapshot(engine)).ToList(),
                globalProperties.GetSnapshot(engine),
                lastUpdateTime
                );
        }

        public void LoadFromSnapshot(IdleEngineSnapshot snapshot)
        {
            Logger.GetLogger().Trace("Loading engine state from snapshot");
            if (snapshot == null)
            {
                return;
            }
            if (snapshot.Entities != null)
            {
                foreach (var entity in snapshot.Entities)
                {
                    if (AllEntities.ContainsKey(entity.EntityKey))
                    {
                        AllEntities[entity.EntityKey].LoadFromSnapshot(entity);
                    }
                }
            }
            if (snapshot.Achievements != null)
            {
                foreach (var achievement in snapshot.Achievements)
                {
                    if (achievements.ContainsKey(achievement.AchievementKey))
                    {
                        achievements[achievement.AchievementKey].LoadFromSnapshot(achievement);
                    }
                }
            }
            var now = System.DateTime.UtcNow;
            var lastUpdate = snapshot.TimeSinceLastUpdate;
            var timeToUpdate = now.Subtract(lastUpdate).TotalSeconds;

            offlinePolicy.Apply(this, (float)timeToUpdate);
        }

        private void setupUi(GameObject root)
        {
            Canvas canvas;
            if(!root.TryGetComponent<Canvas>(out canvas))
            {
                throw new InvalidOperationException("Tried to generate ui, however the component doesn't have a canvas.");
            }
            var rootTabs = uiGenerator.Generate(configuration.UiConfiguration, root, this);
            rootTabs.transform.SetParent(root.transform);
            RectTransform rt = (RectTransform)rootTabs.transform;
            rt.anchoredPosition = Vector3.zero;
            rt.offsetMin = Vector3.zero;
            rt.offsetMax = Vector3.zero;
        }
    }
}