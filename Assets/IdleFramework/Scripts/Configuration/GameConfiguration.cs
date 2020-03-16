using BreakInfinity;
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
        private ISet<EngineHookDefinition> hooks;
        private Dictionary<string, BigDouble> universalCustomEntityProperties;
        private ISet<SingletonEntityDefinition> singletons;
        private Dictionary<string, BigDouble> globalProperties;

        public ISet<EntityDefinition> Entities { get => entities; set => entities = value; }
        public ISet<ModifierDefinitionProperties> Modifiers { get => modifiers;  }
        public ISet<EngineHookDefinition> Hooks { get => hooks; }
        public Dictionary<string, BigDouble> UniversalCustomEntityProperties { get => universalCustomEntityProperties; }
        public IEnumerable<SingletonEntityDefinition> Singletons => singletons;

        public GameConfiguration(ISet<EntityDefinition> entities, ISet<ModifierDefinitionProperties> modifiers, ISet<EngineHookDefinition> hooks, ISet<SingletonEntityDefinition> singletons, Dictionary<string, BigDouble> universalCustomEntityProperties, Dictionary<string, BigDouble> globalProperties)
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
            this.hooks = hooks;
            this.universalCustomEntityProperties = universalCustomEntityProperties;
            this.singletons = singletons;
            this.globalProperties = globalProperties;
        }
    }
}