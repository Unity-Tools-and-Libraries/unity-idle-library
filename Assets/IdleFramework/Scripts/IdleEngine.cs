using BreakInfinity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class IdleEngine
    {
        private float accruedTime;
        private readonly GameConfiguration configuration;
        private readonly Dictionary<string, GameEntity> _allEntities = new Dictionary<string, GameEntity>();
        private readonly ISet<Modifier> _modifiers = new HashSet<Modifier>();
        private readonly Dictionary<string, GameEntity> _resources = new Dictionary<string, GameEntity>();
        private readonly HookManager hooks;

        private readonly Dictionary<string, SingletonEntityDefinition> singletons = new Dictionary<string, SingletonEntityDefinition>();
        private readonly Dictionary<string, ModifiableProperty> globalProperties = new Dictionary<string, ModifiableProperty>();
        private readonly Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

        internal object GetRawGlobalProperty(string propertyName)
        {
            return globalProperties[propertyName].RawValue(this);
        }

        private readonly ISet<Tutorial> tutorials = new HashSet<Tutorial>();

        private bool startHookFired = false;

        private System.Timers.Timer updateThrottleTimer = new System.Timers.Timer(100);
        private ISet<Modifier> lastActiveModifiers = new HashSet<Modifier>();

        public ReadOnlyDictionary<string, GameEntity> AllEntities
        {
            get
            {
                return new ReadOnlyDictionary<string, GameEntity>(_allEntities);
            }
        }

        public ISet<Modifier> Modifiers
        {
            get
            {
                return _modifiers;
            }
        }

        public Dictionary<string, SingletonEntityDefinition> AllSingletons => singletons;

        public IdleEngine(GameConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration must not be null");
            }
            this.configuration = configuration;
            setupEntities();
            setupModifiers();
            hooks = new HookManager(configuration.Hooks, this);
            foreach (SingletonEntityDefinition singleton in configuration.Singletons)
            {
                singletons.Add(singleton.SingletonTypeKey, singleton);
            }
            foreach (AchievementConfiguration achievement in configuration.Achievements.Values)
            {
                achievements.Add(achievement.AchievementKey, new Achievement(achievement));
            }
            foreach (var globalProperty in configuration.GlobalProperties)
            {
                this.globalProperties.Add(globalProperty.Key, new ModifiableProperty(null, globalProperty.Key, globalProperty.Value, this));
            }
            foreach (var tutorial in configuration.Tutorials)
            {
                tutorials.Add(new Tutorial(tutorial));
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
                        Debug.Log(String.Format("Adding {0} from modifier {1} to {2}", effect, modifier.ModifierKey, canBeAffected));
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
                        Debug.Log(String.Format("Adding universtal property {1} to entity {0} with initial value of {2}", entity.EntityKey, customProperty.Key, customProperty.Value));
                        entity.CustomProperties.Add(customProperty.Key, customProperty.Value);
                    }
                }
                GameEntity entityInstance = new GameEntity(entity, this);
                foreach (var otherEntity in configuration.Entities)
                {
                    ValueContainer baseInput;
                    ValueContainer baseOutput;
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

                    entityInstance.ProductionInputs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "inputs-" + otherEntity.EntityKey, baseInput, this);
                    entityInstance.ProductionOutputs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "outputs-" + otherEntity.EntityKey, baseOutput, this);
                    entityInstance.Upkeep[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "upkeep-" + otherEntity.EntityKey, baseUpkeep, this);
                    entityInstance.Costs[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "costs-" + otherEntity.EntityKey, baseCost, this);
                    entityInstance.Requirements[otherEntity.EntityKey] = new ModifiableProperty(entityInstance, "requirements-" + otherEntity.EntityKey, baseRequirement, this);
                }

                _allEntities.Add(entity.EntityKey, entityInstance);
            }
            foreach (GameEntity entity in AllEntities.Values)
            {
                foreach (GameEntity otherEntity in AllEntities.Values)
                {
                    Debug.Log(String.Format("Adding entity {0} as a production source to entity {1}", otherEntity.EntityKey, entity.EntityKey));
                    var modifier = otherEntity.AsModifierEffectFor(this, entity.EntityKey, "production");
                    entity.QuantityChangePerSecond.AddModifierEffect(modifier);
                    _modifiers.Add(modifier.modifier);
                }
            }
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

        internal string GetGlobalStringProperty(string propertyName)
        {
            ModifiableProperty property;
            globalProperties.TryGetValue(propertyName, out property);
            return property != null ? property.GetAsString(this) : "";
        }

        public void BuyEntity(GameEntity gameEntity, BigDouble quantityToBuy, bool buyAllOrNothing)
        {
            if (!gameEntity.CanBeBought)
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
                if (entity.IsActive(this))
                {
                    Debug.Log(String.Format("Updating properties of active entity {0}", entity.EntityKey));
                    entity.ForEachProperty((name, property) =>
                    {
                        property.Update(this, deltaTime);
                    });
                }
                else
                {
                    Debug.Log(String.Format("Skipping inactive entity {0}", entity.EntityKey));
                }
            }
            foreach (var entity in AllEntities.Values)
            {
                if (entity.IsActive(this))
                {
                    entity.QuantityChangePerSecond.Update(this, deltaTime);
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

        internal void PerformProductionForEntity(GameEntity entity, float deltaTime)
        {
            if (entity.ShouldBeDisabled(this))
            {
                return;
            }
            var pendingQuantityChange = hooks.ExecuteEntityProductionHooks(entity);
            entity.ChangeQuantity(pendingQuantityChange);
        }

        private void assertEntityExists(string key)
        {
            if (!_allEntities.ContainsKey(key))
            {
                throw new InvalidProgramException(String.Format("Referenced entity {0} doesn't exist", key));
            }
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

        private void doUpdate(float accruedTime)
        {
            if (!startHookFired)
            {
                hooks.ExecuteEngineStartHooks();
            }
            RecalculateAllEntityProperties(accruedTime);
            foreach (GameEntity entity in _allEntities.Values)
            {
                PerformProductionForEntity(entity, accruedTime);
            }
            foreach (GameEntity entity in _allEntities.Values)
            {
                PerformUpkeepForEntity(entity, accruedTime);
            }
            foreach (Achievement achievement in achievements.Values)
            {
                if (achievement.ShouldBeActive(this))
                {
                    achievement.Gain();
                }
            }
        }
    }
}