using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityQuantityBoughtReference : EntityNumberPropertyReference
    {
        public EntityQuantityBoughtReference(string entityKey) : base(entityKey, "QuantityBought")
        {
        }
    }
}