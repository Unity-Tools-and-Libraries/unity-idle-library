using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    /**
     * Builder class for creating a GameConfiguration.
     */
    public class GameConfigurationBuilder: Builder<GameConfiguration>
    {
        private ISet<EntityDefinition> entities = new HashSet<EntityDefinition>();
        private Dictionary<string, BigDouble> globalProperties = new Dictionary<string, BigDouble>();
        private ISet<ModifierDefinitionProperties> modifiers = new HashSet<ModifierDefinitionProperties>();
        private Dictionary<string, BigDouble> universalCustomEntityProperties = new Dictionary<string, BigDouble>();
        private ISet<EngineHookDefinition> hooks = new HashSet<EngineHookDefinition>();
        private ISet<SingletonEntityDefinition> singletons = new HashSet<SingletonEntityDefinition>();
        private ISet<AchievementConfiguration> achievements = new HashSet<AchievementConfiguration>();

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, BigDouble baseValue)
        {
            globalProperties.Add(propertyName, baseValue);
            return this;
        }

        public GameConfigurationBuilder WithCustomEntityProperty(string customProperty)
        {
            if (EntityDefinition.RESERVED_PROPERTY_NAMES.Contains(customProperty))
            {
                throw new InvalidOperationException(String.Format("Cannot use {0} as a custom property name as it is reserved. Reserved names are {1}", customProperty, EntityDefinition.RESERVED_PROPERTY_NAMES));
            }
            universalCustomEntityProperties.Add(customProperty, 0);
            return this;
        }
        /**
         * Add a definition for a new entity.
         */
        public GameConfigurationBuilder WithEntity(EntityDefinitionBuilder definition)
        {
            var entity = new EntityDefinition(definition);
            entities.Add(entity);
            return this;
        }

        public GameConfigurationBuilder WithSingletonEntity(SingletonEntityDefinitionBuilder singletonEntity)
        {
            var singleton = singletonEntity.Build();
            singletons.Add(singleton);
            return this;
        }

        public GameConfigurationBuilder WithAchievement(AchievementConfigurationBuilder achievement)
        {
            achievements.Add(achievement.Build());
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
            return new GameConfiguration(entities, modifiers, hooks, singletons, universalCustomEntityProperties, globalProperties, achievements);
        }
    }
}