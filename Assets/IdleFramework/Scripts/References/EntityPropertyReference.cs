using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            GameEntity entity;
            if(engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                switch(entityProperty)
                {
                    case "inputs":
                        return entity.ProductionInputs[entitySubProperty].Value;
                    case "outputs":
                        return entity.ProductionOutputs[entitySubProperty].Value;
                    case "costs":
                        return entity.Costs[entitySubProperty].Value;
                    case "requirements":
                        return entity.Requirements[entitySubProperty].Value;
                    case "quantity":
                        return entity.Quantity;
                    case "actual-quantity":
                        return entity.RealQuantity;
                    case "custom":
                        {
                            PropertyReference customPropertyValue;
                            if (entity.CustomProperties.TryGetValue(entitySubProperty, out customPropertyValue))
                            {
                                return customPropertyValue.GetAsNumber(engine);
                            };
                            return 0;
                        }
                    default:
                        Assert.AreEqual(entityProperty, "outputs");
                        throw new InvalidOperationException();
                }
            }
            return 0;
        }

        public override string ToString()
        {
            if(entitySubProperty == null)
            {
                return String.Format("Property {0} of {1}", this.entityProperty, this.entityKey);
            } else
            {
                return String.Format("Property {0}[{2}] of {1}", this.entityProperty, this.entityKey, this.entitySubProperty);
            }
        }
    }
}