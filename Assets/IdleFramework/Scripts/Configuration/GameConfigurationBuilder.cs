using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    /**
     * Builder class for creating a GameConfiguration.
     */
    public class GameConfigurationBuilder
    {
        private ISet<EntityDefinition> entities = new HashSet<EntityDefinition>();
        private ISet<ModifierDefinitionProperties> modifiers = new HashSet<ModifierDefinitionProperties>();
        private Dictionary<string, BigDouble> universalCustomEntityProperties = new Dictionary<string, BigDouble>();

        public GameConfigurationBuilder WithCustomEntityProperty(string customProperty)
        {
            if (EntityDefinition.RESERVED_PROPERTY_NAMES.Contains(customProperty))
            {
                throw new InvalidOperationException(String.Format("Cannot use {0} as a custom property name as it is reserved. Reserved names are {1}", customProperty, EntityDefinition.RESERVED_PROPERTY_NAMES));
            }
            universalCustomEntityProperties.Add(customProperty, 0);
            return this;
        }

        private ISet<EngineHookDefinition> hooks = new HashSet<EngineHookDefinition>();
        /**
         * Add a definition for a new entity.
         */
        public GameConfigurationBuilder WithEntity(EntityDefinitionBuilder definition)
        {
            var entity = new EntityDefinition(definition);
            entities.Add(entity);
            return this;
        }
        /**
         * Finalize and create an unmodifiable GameConfiguration instance.
         */

        public GameConfigurationBuilder WithModifier(ModifierDefinitionProperties modifier)
        {
            foreach(var existingModifier in modifiers)
            {
                if (existingModifier.ModifierKey == modifier.ModifierKey)
                {
                    throw new InvalidOperationException(String.Format("Duplicate modifier {0} found", modifier.ModifierKey));
                }
            }
            if (modifier is ModifierDefinitionBuilder builder)
            {
                modifiers.Add(builder.Build());
            }
            else
            {
                modifiers.Add(modifier);
            }
            return this;
        }

        public GameConfigurationBuilder WithHook(EngineHookConfigurationBuilder hook)
        {
            hooks.Add(hook.Build());
            return this;
        }

        public GameConfiguration Build()
        {
            return new GameConfiguration(entities, modifiers, hooks, universalCustomEntityProperties);
        }
    }
}