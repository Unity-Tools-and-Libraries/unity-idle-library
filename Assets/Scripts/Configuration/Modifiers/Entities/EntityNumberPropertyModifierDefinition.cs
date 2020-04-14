using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityNumberPropertyModifierDefinition : EntityModifierDefinition<BigDouble>
    {
        private readonly string entityKey;
        private readonly string entityPropertyPath;
        private readonly NumberContainer modifierValue;
        private readonly StringContainer modifierOperators;

        public EntityNumberPropertyModifierDefinition(string entityKey, string entityPropertyPath, NumberContainer modifierValue, StringContainer modifierOperators)
        {
            this.entityKey = entityKey;
            this.entityPropertyPath = entityPropertyPath;
            this.modifierValue = modifierValue;
            this.modifierOperators = modifierOperators;
        }

        public override BigDouble CalculateEffect(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public override void Update(IdleEngine engine, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}