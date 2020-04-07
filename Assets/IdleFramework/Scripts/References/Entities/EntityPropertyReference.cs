using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace IdleFramework
{
    public class EntityPropertyReference : PropertyReference
    {
        private string entityKey;
        private string entityProperty;

        public EntityPropertyReference(string entityKey, string entityProperty)
        {
            this.entityKey = entityKey;
            this.entityProperty = entityProperty;
        }

        public BigDouble GetAsNumber(IdleEngine engine)
        {
            object value = engine.AllEntities.ContainsKey(entityKey) ? engine.AllEntities[entityKey] : null;
            value = engine.Traverse(value, entityProperty);
            if(value != null && typeof(ModifiableProperty) == value.GetType())
            {
                return (value as ModifiableProperty).GetAsNumber(engine);
            }
            return value != null ? (BigDouble)value : 0;
        }

        public bool GetAsBoolean(IdleEngine engine)
        {
            GameEntity entity;
            if (engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetPropertyAsBoolean(entityProperty);
            }
            return false;
        }

        public override string ToString()
        {
            return String.Format("Property {0} of {1}", this.entityProperty, this.entityKey);
        }

        public string GetAsString(IdleEngine engine)
        {
            GameEntity entity;
            if (engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetPropertyAsString(entityProperty);
            }
            return "";
        }

        public object RawValue(IdleEngine engine)
        {
            GameEntity entity;
            if (engine.AllEntities.TryGetValue(entityKey, out entity))
            {
                return entity.GetRawProperty(entityProperty);
            }
            return null;
        }

        public PropertyContainer GetAsContainer(IdleEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}