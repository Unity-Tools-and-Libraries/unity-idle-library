using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IdleFramework
{
    public class EntityPropertyReference: PropertyReference
    {
        private string entityKey;
        private string entityProperty;
        private string entitySubProperty;

        public EntityPropertyReference(string entityKey, string entityProperty, string entitySubProperty)
        {
            this.entityKey = entityKey;
            this.entityProperty = entityProperty;
            this.entitySubProperty = entitySubProperty;
        }

        public EntityPropertyReference(string entityKey, string entityProperty): this(entityKey, entityProperty, null)
        {

        }

        public BigDouble Get(IdleEngine engine)
        {
            GameEntity entity;
            if(engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                switch(entityProperty)
                {
                    case "inputs":
                        return entity.ProductionInputs[entitySubProperty];
                    case "ouputs":
                        return entity.ProductionOutputs[entitySubProperty];
                    case "costs":
                        return entity.Costs[entitySubProperty];
                    case "requirements":
                        return entity.Requirements[entitySubProperty];
                    case "quantity":
                        return entity.Quantity;
                    case "actual-quantity":
                        return entity.RealQuantity;
                }
            }
            return 0;
        }
    }
}