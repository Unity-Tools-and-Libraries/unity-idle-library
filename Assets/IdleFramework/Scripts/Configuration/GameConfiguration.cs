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
        private readonly ISet<EntityDefinition> entities;
        private readonly ISet<ModifierDefinitionProperties> modifiers;
        private readonly HooksContainer hooks;
        private readonly Dictionary<string, ValueContainer> sharedEntityProperties;
        private readonly ISet<SingletonEntityDefinition> singletons;
        private readonly Dictionary<string, ValueContainer> globalProperties;
        private readonly Dictionary<string, GlobalSingletonPropertyDefinition> globalSingletonProperties;
        private readonly Dictionary<string, AchievementConfiguration> achievements = new Dictionary<string, AchievementConfiguration>();
        private readonly ISet<TutorialConfiguration> tutorials;
        private readonly Dictionary<string, string> strings;

        public ISet<EntityDefinition> Entities { get => entities; }
        public ISet<ModifierDefinitionProperties> Modifiers { get => modifiers;  }
        public HooksContainer Hooks => hooks;
        public Dictionary<string, ValueContainer> SharedEntityProperties => sharedEntityProperties;
        public IEnumerable<SingletonEntityDefinition> Singletons => singletons;
        public Dictionary<string, AchievementConfiguration> Achievements { get => achievements; }
        public Dictionary<string, ValueContainer> GlobalProperties => globalProperties;

        public ISet<TutorialConfiguration> Tutorials => tutorials;

        public Dictionary<string, string> Strings => strings;

        public Dictionary<string, GlobalSingletonPropertyDefinition> GlobalSingletonProperties => globalSingletonProperties;

        public GameConfiguration(ISet<EntityDefinition> entities, 
            ISet<ModifierDefinitionProperties> modifiers, 
            HookConfigurationBuilder hooks, 
            ISet<SingletonEntityDefinition> singletons, 
            Dictionary<string, ValueContainer> sharedCustomEntityProperties, 
            Dictionary<string, ValueContainer> globalProperties,
            Dictionary<string, GlobalSingletonPropertyDefinition> globalSingletonProperties,
            ISet<AchievementConfiguration> achievements, 
            ISet<TutorialConfiguration> tutorials,
            Dictionary<string, string> strings)
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
            this.hooks = hooks.Build();
            this.sharedEntityProperties = sharedCustomEntityProperties;
            this.singletons = singletons;
            this.globalProperties = globalProperties;
            this.tutorials = tutorials;
            foreach(var achievement in achievements)
            {
                this.achievements.Add(achievement.AchievementKey, achievement);
            }
            this.strings = strings;
            this.globalSingletonProperties = globalSingletonProperties;
        }
    }
}