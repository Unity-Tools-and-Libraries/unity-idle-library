using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Configuration
{
    public class EntityNumberPropertyModifierDefinition : EntityModifierDefinition
    {
        private readonly string entityKey;
        private readonly string entityPropertyPath;
        private readonly EffectType operation;
        private readonly NumberContainer operand;
        private readonly StateMatcher isActiveMatcher;

        public EntityNumberPropertyModifierDefinition(string entityKey, string entityPropertyPath, EffectType operation, NumberContainer operand, StateMatcher activeWhenMatcher)
        {
            this.entityKey = entityKey;
            this.entityPropertyPath = entityPropertyPath;
            this.operation = operation;
            this.operand = operand;
            this.isActiveMatcher = new EntityBooleanPropertyMatcher(entityKey, "Enabled", true)
                .And(new EntityNumberPropertyMatcher(entityKey, "Quantity", Comparison.GREATER_THAN_OR_EQUAL, Literal.Of(1)))
                .And(activeWhenMatcher);
        }

        public EntityNumberPropertyModifierDefinition(string entityKey, string entityPropertyPath, EffectType operation, NumberContainer operand) : this(entityKey, entityPropertyPath, operation, operand, Always.Instance)
        {
            
        }

        public string EntityKey => entityKey;

        public string EntityPropertyPath => entityPropertyPath;

        public EffectType Operation => operation;

        public NumberContainer Operand => operand;

        public override StateMatcher IsActiveMatcher => isActiveMatcher;
    }
}