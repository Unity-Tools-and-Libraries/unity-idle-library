using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityReference : ValueContainer
    {
        private readonly string entityKey;

        public EntityReference(string entityKey)
        {
            this.entityKey = entityKey;
        }

        public Entity Get(IdleEngine engine)
        {
            return engine.GetEntity(entityKey);
        }
    }
}