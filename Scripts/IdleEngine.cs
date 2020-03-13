using UnityEngine;
using System.Collections.Generic;
using BreakInfinity;
using System;

namespace IdleFramework
{
    public class IdleEngine : Updates
    {
        private readonly GameConfiguration configuration;
        private readonly Dictionary<string, GameEntity> _allEntities = new Dictionary<string, GameEntity>();
        private readonly ISet<ModifierDefinition> _modifiers = new HashSet<ModifierDefinition>();
        private readonly Dictionary<string, GameEntity> _resources = new Dictionary<string, GameEntity>();
        private bool updatedThrottled = false;
        private System.Timers.Timer updateThrottleTimer = new System.Timers.Timer(100);
        private ISet<ModifierDefinition> lastActiveModifiers;

        public ReadOnlyDictionary<string, GameEntity> AllEntities
        {
            get
            {
                return new ReadOnlyDictionary<string, GameEntity>(_allEntities);
            }
        }

        public ISet<ModifierDefinition> Modifiers
        {
            get
            {
                return _modifiers;
            }
        }

        public IdleEngine(GameConfiguration configuration)
        {
            updateThrottleTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                this.updatedThrottled = false;
            };
            foreach (EntityDefinition entity in configuration.Entities)
            {
                GameEntity entityInstance = new GameEntity(entity, this);
                if (entity.Types.Contains("resource"))
                {
                    _resources.Add(entity.EntityKey, entityInstance);
                }
                _allEntities.Add(entity.EntityKey, entityInstance);
            }
            foreach (ModifierDefinition modifier in configuration.Modifiers)
            {
                _modifiers.Add(modifier);
            }
        }

        public BigDouble GetProductionForEntity(string entityKey)
        {
            BigDouble totalProduction = 0;
            foreach(var entity in AllEntities.Values)
            {
                BigDouble entityProduction = 0;
                if(entity.ProductionOutputs.TryGetValue(entityKey, out entityProduction)) {
                    totalProduction += entityProduction * entity.Quantity;
                }
                if(entity.ProductionInputs.TryGetValue(entityKey, out entityProduction))
                {
                    totalProduction -= entityProduction * entity.Quantity;
                }
                if(entity.Upkeep.TryGetValue(entityKey, out entityProduction))
                {
                    totalProduction -= entityProduction * entity.Quantity;
                }
            }
            return totalProduction;
        }

        public void BuyEntity(GameEntity gameEntity, BigDouble quantityToBuy, bool buyAllOrNothing)
        {
            if(!gameEntity.CanBeBought)
            {
                return;
            }
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
                UpdateResourcesFromEntityPurchase(gameEntity, quantityToBuy);
            }
        }

        private void RecalculateAllEntityProperties()
        {
            ISet<ModifierDefinition> activeModifiers = new HashSet<ModifierDefinition>();
            foreach (var modifier in _modifiers)
            {
                if (modifier.IsActive(this))
                {
                    activeModifiers.Add(modifier);
                }
            }
            lastActiveModifiers = activeModifiers;
            foreach (var entity in AllEntities.Values)
            {
                entity.Costs.Clear();
                foreach (var cost in entity.BaseCosts)
                {
                    entity.Costs.Add(cost.Key, cost.Value.Get(this));
                }
                entity.Requirements.Clear();
                foreach (var requirement in entity.BaseRequirements)
                {
                    entity.Requirements.Add(requirement.Key, requirement.Value.Get(this));
                }
                entity.ProductionOutputs.Clear();
                foreach (var output in entity.BaseProductionOutputs)
                {
                    entity.ProductionOutputs.Add(output.Key, output.Value.Get(this));
                }
                entity.ProductionInputs.Clear();
                foreach (var input in entity.BaseProductionInputs)
                {
                    entity.ProductionInputs.Add(input.Key, input.Value.Get(this));
                }
                entity.Upkeep.Clear();
                foreach(var upkeep in entity.BaseUpkeep)
                {
                    entity.Upkeep.Add(upkeep.Key, upkeep.Value.Get(this));
                }
                entity.MinimumProductionOutputs.Clear();
                foreach(var minProduction in entity.BaseMinimumProductionOutputs)
                {
                    entity.MinimumProductionOutputs.Add(minProduction.Key, minProduction.Value.Get(this));
                }
            }
            List<EntityEffect> effectsToApply = new List<EntityEffect>();
            foreach (var modifier in activeModifiers)
            {
                foreach (var effect in modifier.Effects)
                {
                    effectsToApply.Add(effect);
                }
            }
            effectsToApply.Sort((a, b) =>
            {
                if (a.GetType() == b.GetType())
                {
                    return 0;
                }
                return 0;
            });
            foreach (var effect in effectsToApply)
            {
                effect.ApplyEffect(this);
            }

        }

        public BigDouble PerformUpkeepForEntity(GameEntity entity)
        {
            if (entity.ShouldBeDisabled(this))
            {
                return 0;
            }
            BigDouble quantity = entity.RealQuantity;
            // Determine max number can be supported.
            foreach (var resource in entity.Upkeep)
            {
                var maxQuantityPossible = resource.Value > 0 ? BigDouble.Floor(_allEntities[resource.Key].Quantity / resource.Value) : quantity;
                quantity = BigDouble.Min(quantity, maxQuantityPossible);
            }
            // Consume upkeep requirements
            foreach (var resource in entity.Upkeep)
            {
                _allEntities[resource.Key].ChangeQuantity(-resource.Value * quantity);
            }
            // Set new quantity
            entity.SetQuantity(quantity);
            return quantity;
        }

        internal void PerformProductionForEntity(GameEntity entity)
        {
            if (entity.ShouldBeDisabled(this))
            {
                return;
            }
            var quantityProducing = entity.Quantity;
            // Determine the quantity able to produce.
            foreach (var resource in entity.ProductionInputs)
            {
                assertEntityExists(resource.Key);

                if (resource.Value == 0 || _allEntities[resource.Key].ShouldBeDisabled(this))
                {
                    continue;
                }
                var maxQuantityPossible = BigDouble.Floor(_allEntities[resource.Key].Quantity / resource.Value);
                quantityProducing = (maxQuantityPossible < quantityProducing && !entity.ScaleProductionOnAvailableInputs) ? 0 : BigDouble.Min(maxQuantityPossible, quantityProducing);
                if (quantityProducing == 0)
                {
                    break;
                }

            }

            // Consume inputs
            foreach (var resource in entity.ProductionInputs)
            {
                _allEntities[resource.Key].ChangeQuantity(-resource.Value * quantityProducing);
            }
            var entitiesWhichProduced = new HashSet<string>();
            // Produce outputs
            foreach (var resource in entity.ProductionOutputs)
            {
                assertEntityExists(resource.Key);
                if (AllEntities[resource.Key].ShouldBeDisabled(this))
                {
                    continue;
                }
                var calculatedQuantity = resource.Value * quantityProducing;
                BigDouble minimumQuantity = 0;
                entity.MinimumProductionOutputs.TryGetValue(resource.Key, out minimumQuantity);
                var quantityToProduce = BigDouble.Max(calculatedQuantity, minimumQuantity);
                _allEntities[resource.Key].ChangeQuantity(resource.Value * quantityProducing);
            }

            foreach (var resource in entity.MinimumProductionOutputs)
            {
                assertEntityExists(resource.Key);
                if (entitiesWhichProduced.Contains(resource.Key) || AllEntities[resource.Key].ShouldBeDisabled(this))
                {
                    continue;
                }
                _allEntities[resource.Key].ChangeProgress(resource.Value);
            }
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
        public void Update()
        {
            if (!updatedThrottled)
            {
                updatedThrottled = true;
                updateThrottleTimer.Start();
                RecalculateAllEntityProperties();
                foreach (GameEntity entity in _allEntities.Values)
                {
                    PerformProductionForEntity(entity);
                }
                foreach (GameEntity entity in _allEntities.Values)
                {
                    PerformUpkeepForEntity(entity);
                }
            }
        }

        internal void UpdateResourcesFromEntityProduction(string entityKey, BigDouble quantityProducing)
        {
            var entity = _allEntities[entityKey];
            UpdateResourcesFromEntityConsumption(entity, quantityProducing);
        }

        internal void UpdateResourcesFromEntityConsumption(GameEntity entity, BigDouble quantityConsuming)
        {
            foreach (var consumed in entity.Upkeep)
            {
                var totalConsumed = consumed.Value * quantityConsuming;
                AllEntities[consumed.Key].ChangeQuantity(-totalConsumed);
            }
        }
    }
}