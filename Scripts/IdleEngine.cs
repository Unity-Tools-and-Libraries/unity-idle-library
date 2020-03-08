using UnityEngine;
using System.Collections.Generic;
using BreakInfinity;

namespace IdleFramework
{
    public class IdleEngine: Updates
    {
        private readonly GameConfiguration configuration;
        private readonly Dictionary<string, GameEntity> _allEntities = new Dictionary<string, GameEntity>();
        public ReadOnlyDictionary<string, GameEntity> AllEntities
        {
            get
            {
                return new ReadOnlyDictionary<string, GameEntity>(_allEntities);
            }
        }
        private readonly Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
        public ReadOnlyDictionary<string, Resource> Resources
        {
            get
            {
                return new ReadOnlyDictionary<string, Resource>(_resources);
            }
        }

        private bool updatedThrottled = false;
        private System.Timers.Timer updateThrottleTimer = new System.Timers.Timer(100);

        public IdleEngine(GameConfiguration configuration)
        {
            updateThrottleTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                this.updatedThrottled = false;
            };
            foreach (EntityDefinition resource in configuration.Resources.Values)
            {
                if (resource.Types.Contains("resource"))
                {
                    _resources.Add(resource.EntityKey, new Resource(resource, this));
                }
            }
        }

        public void AddModifierToEntity(string entityKey, Modifier modifier)
        {
            GameEntity entity = null;
            if(_allEntities.TryGetValue(entityKey, out entity))
            {
                entity.AddModifier(modifier);
            }
        }

        /**
         *Change the quantity of the resource with the given key, by the given amount.
         * Returns the quantity of the change or 0 if the entity doesn't exist.
         */
        public BigDouble ChangeEntityQuantity(string entityKey, BigDouble quantityToAdd)
        {
            GameEntity entity= null;
            if (_allEntities.TryGetValue(entityKey, out entity))
            {
                entity.ChangeQuantity(quantityToAdd);
                return quantityToAdd;
            } else
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
                foreach (Resource r in _resources.Values)
                {
                    r.Update();
                }
            }
        }

        internal void UpdateResourcesFromEntityProduction(Dictionary<string, BigDouble> productionResults)
        {
            foreach (var produced in productionResults)
            {
                Resources[produced.Key].ChangeProgress(produced.Value);
            }
        }

        internal void UpdateResourcesFromEntityConsumption(GameEntity entity, BigDouble quantityConsuming)
        {
            foreach (var consumed in entity.Consumes)
            {
                var totalConsumed = consumed.Value.Quantity * quantityConsuming;
                Resources[consumed.Key].ChangeQuantity(-totalConsumed);
            }
        }

    }
}