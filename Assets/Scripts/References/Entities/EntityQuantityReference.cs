using BreakInfinity;
using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityQuantityReference : NumberContainer
    {
        private readonly string entityName;

        public EntityQuantityReference(string entityName)
        {
            this.entityName = entityName;
        }

        public BigDouble Get(IdleEngine engine)
        {
            return engine.GetEntity(entityName).Quantity;
        }
    }
}