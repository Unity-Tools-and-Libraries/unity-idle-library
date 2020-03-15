using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class ModifierDefinitionBuilder: ModifierDefinitionProperties, Builder<ModifierDefinition>
    {
        private string modifierKey;
        private StateMatcher stateMatcher;
        private ISet<EntityEffect> entityEffects = new HashSet<EntityEffect>();
        public string ModifierKey => modifierKey;

        public ModifierDefinitionBuilder(string modifierKey)
        {
            this.modifierKey = modifierKey;
        }

        public ModifierDefinitionBuilder DoesNothing()
        {
            entityEffects.Add(new NothingEntityEffect());
            return this;
        }

        public ModifierDefinitionBuilder HasEntityEffect(EntityEffect effect)
        {
            entityEffects.Add(effect);
            return this;
        }

        public ModifierDefinition Build()
        {
            if(stateMatcher == null)
            {
                throw new InvalidOperationException("A state matcher must be added to determine when to apply the effect.");
            }
            if(entityEffects.Count == 0)
            {
                throw new InvalidOperationException("At least one effect must be added for the effect to do anything.");
            }
            return new ModifierDefinition(ModifierKey, stateMatcher, entityEffects);
        }

        public ModifierDefinitionBuilder StateMatcherMatches(StateMatcher matcher)
        {
            return this;
        }

        /*
         * Configure when the effect is active.
         */
        public EffectTriggerDefinitionConfigurer Active()
        {
            return new EffectTriggerDefinitionConfigurer(this);
        }

        public class EffectTriggerDefinitionConfigurer
        {
            private ModifierDefinitionBuilder parent;

            internal EffectTriggerDefinitionConfigurer(ModifierDefinitionBuilder parent)
            {
                this.parent = parent;
            }

            public EffectTriggerDefinitionConfigurer Always()
            {
                parent.stateMatcher = new Always();
                return this;
            }

            public ModifierDefinition Build()
            {
                return parent.Build();
            }

            public ModifierDefinitionBuilder And(){
                return parent;
            }

            public EffectTriggerDefinitionConfigurer When(EntityPropertyMatcher entityPropertyMatcher)
            {
                parent.stateMatcher = entityPropertyMatcher;
                return this;
            }
        }
    }
}