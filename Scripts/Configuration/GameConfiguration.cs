using System;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    /**
     * Provides configuration for an instance of IdleEngine.
     **/
    public class GameConfiguration
    {
        private ISet<EntityDefinition> entities;
        private ISet<ModifierDefinitionProperties> modifiers;

        public ISet<EntityDefinition> Entities { get => entities; set => entities = value; }
        public ISet<ModifierDefinitionProperties> Modifiers { get => modifiers;  }

        public GameConfiguration(ISet<EntityDefinition> entities, ISet<ModifierDefinitionProperties> modifiers)
        {
            var entityKeys = new HashSet<string>();
            foreach(var entityDefinition in entities)
            {
                if (!entityKeys.Add(entityDefinition.EntityKey))
                {
                    throw new ArgumentException(String.Format("The key {0} was used multiple times.", entityDefinition.EntityKey));
                }
            }
            this.entities = entities;
            this.modifiers = modifiers;
        }

        public GameConfiguration(ISet<EntityDefinition> entities) : this(entities, new HashSet<ModifierDefinitionProperties>())
        {
            
        }
    }
    /**
     * Builder class for creating a GameConfiguration.
     */
    public class GameConfigurationBuilder
    {
        private ISet<EntityDefinition> entities = new HashSet<EntityDefinition>();
        private ISet<ModifierDefinitionProperties> modifiers = new HashSet<ModifierDefinitionProperties>();

        public GameConfigurationBuilder AddEffect(ModifierDefinition effectDefinitionBuilder)
        {
            return this;
        }

        /**
         * Add a definition for a new entity.
         */
        public GameConfigurationBuilder WithEntity(EntityDefinition definition)
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

        public GameConfiguration Build()
        {
            return new GameConfiguration(entities, modifiers);
        }
    }
}