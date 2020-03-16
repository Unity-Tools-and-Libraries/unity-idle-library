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
        private ISet<ModifierDefinitionProperties> modifiers = new HashSet<ModifierDefinitionProperties>();
        private ISet<EngineHookDefinition> hooks = new HashSet<EngineHookDefinition>();
        private ISet<SingletonEntityDefinition> singletons = new HashSet<SingletonEntityDefinition>();
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
            return new GameConfiguration(entities, modifiers, hooks, singletons);
        }
    }
}