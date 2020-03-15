using UnityEngine;
using System.Collections.Generic;
using BreakInfinity;
using System;

namespace IdleFramework
{
    public class IdleEngine
    {
        private readonly GameConfiguration configuration;
        private readonly Dictionary<string, GameEntity> _allEntities = new Dictionary<string, GameEntity>();
        private readonly ISet<ModifierDefinition> _modifiers = new HashSet<ModifierDefinition>();
        private readonly Dictionary<string, GameEntity> _resources = new Dictionary<string, GameEntity>();
        private readonly Dictionary<EngineHookAction, Dictionary<string, List<EngineHookDefinition>>> hooks = new Dictionary<EngineHookAction, Dictionary<string, List<EngineHookDefinition>>>();

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
            foreach (EngineHookDefinition hook in configuration.Hooks)
            {
                Dictionary<string, List<EngineHookDefinition>> hooksForAction;
                if (!hooks.TryGetValue(hook.Selector.Action, out hooksForAction))
                {
                    hooks.Add(hook.Selector.Action, new Dictionary<string, List<EngineHookDefinition>>());
                }
                List<EngineHookDefinition> hooksForActor;
                if (!hooks[hook.Selector.Action].TryGetValue(hook.Selector.Actor, out hooksForActor))
                {
                    hooksForActor = new List<EngineHookDefinition>();
                    hooks[hook.Selector.Action].Add(hook.Selector.Actor, hooksForActor);
                }
                hooksForActor.Add(hook);
            }
        }

        public BigDouble GetProductionForEntity(string entityKey)
        {
            assertEntityExists(entityKey);
            var queriedEntity = AllEntities[entityKey];
            if (queriedEntity.ShouldBeDisabled(this))
            {
                return 0;
            }
            BigDouble totalProduction = 0;
            foreach (var entity in AllEntities.Values)
            {
                GameEntityProperty entityProduction;
                if (entity.ProductionOutputs.TryGetValue(entityKey, out entityProduction))
                {
                    totalProduction += entityProduction.Value * entity.Quantity;
                }
                if (entity.ProductionInputs.TryGetValue(entityKey, out entityProduction))
                {
                    totalProduction -= entityProduction.Value * entity.Quantity;
                }
                if (entity.Upkeep.TryGetValue(entityKey, out entityProduction))
                {
                    totalProduction -= entityProduction.Value * entity.Quantity;
                }
            }
            return totalProduction;
        }

        public void BuyEntity(GameEntity gameEntity, BigDouble quantityToBuy, bool buyAllOrNothing)
        {
            if (!gameEntity.CanBeBought)
            {
                return;
            }
            foreach (var resource in gameEntity.Costs)
            {
                var maxPurchaseable = resource.Value.Value == 0 ? quantityToBuy : BigDouble.Floor(_allEntities[resource.Key].Quantity / resource.Value.Value * quantityToBuy);
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
                foreach (var upkeep in entity.BaseUpkeep)
                {
                    entity.Upkeep.Add(upkeep.Key, upkeep.Value.Get(this));
                }
                entity.MinimumProductionOutputs.Clear();
                foreach (var minProduction in entity.BaseMinimumProductionOutputs)
                {
                    entity.MinimumProductionOutputs.Add(minProduction.Key, minProduction.Value.Get(this));
                }
            }
            List<ModifierAndEffect> effectsToApply = new List<ModifierAndEffect>();
            foreach (var modifier in activeModifiers)
            {
                foreach (var effect in modifier.Effects)
                {
                    effectsToApply.Add(new ModifierAndEffect(modifier, effect));
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
                effect.effect.ApplyEffect(this, effect.modifier);
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
                var maxQuantityPossible = resource.Value.Value > 0 ? BigDouble.Floor(_allEntities[resource.Key].Quantity / resource.Value.Value) : quantity;
                quantity = BigDouble.Min(quantity, maxQuantityPossible);
            }
            // Consume upkeep requirements
            foreach (var resource in entity.Upkeep)
            {
                _allEntities[resource.Key].ChangeQuantity(-resource.Value.Value * quantity);
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
            var inputsToConsume = new Dictionary<String, BigDouble>();
            var outputsToProduce = new Dictionary<string, BigDouble>();

            // Consume inputs
            foreach (var resource in entity.ProductionInputs)
            {
                inputsToConsume.Add(resource.Key, resource.Value.Value * quantityProducing);
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
                var calculatedQuantity = resource.Value.Value * quantityProducing;
                GameEntityProperty minimumQuantity;
                BigDouble quantityToProduce = 0;
                if (entity.MinimumProductionOutputs.TryGetValue(resource.Key, out minimumQuantity))
                {
                    quantityToProduce = BigDouble.Max(calculatedQuantity, minimumQuantity);
                }
                outputsToProduce.Add(resource.Key, resource.Value.Value * quantityProducing);
            }

            foreach (var resource in entity.MinimumProductionOutputs)
            {
                assertEntityExists(resource.Key);
                if (entitiesWhichProduced.Contains(resource.Key) || AllEntities[resource.Key].ShouldBeDisabled(this))
                {
                    continue;
                }
                outputsToProduce.Add(resource.Key, resource.Value.Value);
            }
            var entityProductionResult = new EntityProductionResult(entity, inputsToConsume, outputsToProduce);
            entityProductionResult = executeHooks<EntityProductionResult>(EngineHookAction.WILL_PRODUCE, entityProductionResult);
            applyProductionResult(entityProductionResult);
        }

        private void applyProductionResult(EntityProductionResult entityProductionResult)
        {
            var consumed = entityProductionResult.InputsToConsume;
            var produced = entityProductionResult.OutputsToProduce;
            foreach (var resource in consumed)
            {
                AllEntities[resource.Key].ChangeQuantity(-resource.Value);
            }
            foreach(var resource in produced)
            {
                AllEntities[resource.Key].ChangeQuantity(resource.Value);
            }
        }

        private T executeHooks<T>(EngineHookAction action, HookExecutionContext<T> executionContext)
        {
            List<EngineHookDefinition> generalHooks = null;
            List<EngineHookDefinition> specificHooks = null;
            switch (action)
            {
                case EngineHookAction.WILL_PRODUCE
                :
                    Dictionary<string, List<EngineHookDefinition>> actionHooks;
                    if (hooks.TryGetValue(action, out actionHooks))
                    {
                        actionHooks.TryGetValue("*", out generalHooks);
                        actionHooks.TryGetValue(executionContext.Actor, out specificHooks);
                    }
                    break;
            }
            var payload = executionContext.Payload;
            if (generalHooks != null)
            {
                foreach (var hook in generalHooks)
                {
                    payload = (T)hook.Execute(payload);
                }
            }
            if (specificHooks != null)
            {
                foreach (var hook in specificHooks)
                {
                    payload = (T)hook.Execute(payload);
                }
            }
            return payload;
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
                var totalConsumed = consumed.Value.Value * quantityConsuming;
                AllEntities[consumed.Key].ChangeQuantity(-totalConsumed);
            }
        }
    }
}