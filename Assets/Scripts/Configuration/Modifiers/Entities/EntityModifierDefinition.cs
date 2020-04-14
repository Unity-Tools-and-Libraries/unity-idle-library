using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    /*
     * A modifier which applies to an entity.
     */
    public abstract class EntityModifierDefinition<T> : ModifierDefinition<T>
    {
        public abstract T CalculateEffect(IdleEngine engine);
        public abstract void Update(IdleEngine engine, float deltaTime);
    }
}