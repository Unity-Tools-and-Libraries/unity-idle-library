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
            this.entityKey = entityKey.ToLower();
            this.entityProperty = entityProperty.ToLower();
            this.entitySubProperty = entitySubProperty.ToLower();
        }

        public EntityPropertyReference(string entityKey, string entityProperty): this(entityKey, entityProperty, null)
        {

        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            GameEntity entity;
            if(engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetPropertyAsNumber(entityProperty, entitySubProperty);
            }
            return 0;
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            GameEntity entity;
            if (engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetPropertyAsBoolean(entityProperty, entitySubProperty);
            }
            return false;
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

        public string GetAsString(IdleEngine engine)
        {
            GameEntity entity;
            if (engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetPropertyAsString(entityProperty, entitySubProperty);
            }
            return "";
        }

        public object RawValue(IdleEngine engine)
        {
            GameEntity entity;
            if (engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetRawProperty(entityProperty, entitySubProperty);
            }
            return null;
        }
    }
}