using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace IdleFramework
{
    public class EntityNumberPropertyReference : NumberContainer, EntityPropertyReference
    {
        private string entityKey;
        private string entityProperty;

        public string EntityKey => entityKey;

        public string PropertyName => entityProperty;

        public EntityNumberPropertyReference(string entityKey, string entityProperty)
        {
            this.entityKey = entityKey;
            this.entityProperty = entityProperty;
        }
        public override string ToString()
        {
            return String.Format("Property {0} of {1}", this.entityProperty, this.entityKey);
        }

        public BigDouble Get(IdleEngine engine)
        {
            return engine.GetEntity(entityKey).GetNumberProperty(entityProperty);
        }
    }
}