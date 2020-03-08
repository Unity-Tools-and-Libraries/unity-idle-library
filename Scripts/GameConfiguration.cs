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
        public ReadOnlyDictionary<string, EntityDefinition> Resources;
        internal GameConfiguration(ISet<EntityDefinition> entities)
        {
            var resourceDefinitions = new Dictionary<string, EntityDefinition>();
            foreach(EntityDefinition entity in entities)
            {
                if(entity.Types.Contains("resource"))
                {
                    resourceDefinitions.Add(entity.EntityKey, entity);
                }
            }
            Resources = new ReadOnlyDictionary<string, EntityDefinition>(resourceDefinitions);
        }
    }
    /**
     * Builder class for creating a GameConfiguration.
     */
    public class GameConfigurationBuilder
    {
        private ISet<EntityDefinition> entities = new HashSet<EntityDefinition>();

        /**
         * Add a definition for a new resource.
         */
        public GameConfigurationBuilder AddEntityDefinition(EntityDefinitionProperties definition)
        {
            var entity = new EntityDefinition(definition);
            entities.Add(entity);
            return this;
        }
        /**
         * Finalize and create an unmodifiable GameConfiguration instance.
         */
        public GameConfiguration Build()
        {
            return new GameConfiguration(entities);
        }

        internal object AddEntityDefinition(object p)
        {
            throw new NotImplementedException();
        }
    }

}