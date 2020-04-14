using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IdleFramework
{
    public class EntityStringPropertyReference : EntityPropertyReference, StringContainer
    {
        private readonly string entityKey;
        private readonly string propertyName;

        public EntityStringPropertyReference(string entityKey, string propertyName)
        {
            this.entityKey = entityKey;
            this.propertyName = propertyName;
        }

        public string EntityKey => entityKey;

        public string PropertyName => propertyName;

        public string Get(IdleEngine engine)
        {
            return engine.GetEntity(entityKey).GetStringProperty(propertyName);
        }
    }
}