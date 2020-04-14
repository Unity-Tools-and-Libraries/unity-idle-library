using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityBooleanPropertyReference : BooleanContainer, EntityPropertyReference
    {
        private readonly string entityKey;
        private readonly string propertyName;

        public EntityBooleanPropertyReference(string entityKey, string propertyName)
        {
            this.entityKey = entityKey;
            this.propertyName = propertyName;
        }

        public string EntityKey => entityKey;

        public string PropertyName => propertyName;

        public bool Get(IdleEngine engine)
        {
            return engine.GetEntity(entityKey).GetBooleanProperty(propertyName);
        }
    }
}