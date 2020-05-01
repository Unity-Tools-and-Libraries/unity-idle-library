using BreakInfinity;
using IdleFramework.UI.Components;
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
        private readonly Dictionary<string, EntityDefinition> entities = new Dictionary<string, EntityDefinition>();
        private readonly HooksConfigurationContainer hooks;

        private readonly Dictionary<string, AchievementConfiguration> achievements = new Dictionary<string, AchievementConfiguration>();
        private readonly ISet<TutorialConfiguration> tutorials;
        private readonly Dictionary<string, string> strings;
        private PropertyHolder globalProperties;
        private PropertyHolder commonEntityProperties;
        private ISet<AchievementConfiguration> achievements1;
        private Dictionary<string, string> externalizedStrings;
        private IOfflinePolicy offlinePolicy;
        private UiConfiguration uiConfiguration;

        public Dictionary<string, EntityDefinition> Entities => entities;
        public HooksConfigurationContainer Hooks => hooks;
        public Dictionary<string, AchievementConfiguration> Achievements => achievements;

        public ISet<TutorialConfiguration> Tutorials => tutorials;

        public Dictionary<string, string> Strings => strings;

        public IDictionary<string, StringContainer> GlobalStringProperties => globalProperties.StringProperties;
        public IDictionary<string, NumberContainer> GlobalNumberProperties => globalProperties.NumberProperties;
        public IDictionary<string, BooleanContainer> GlobalBooleanProperties => globalProperties.BooleanProperties;

        public IDictionary<string, StringContainer> CommonEntityStringProperties => commonEntityProperties.StringProperties;
        public IDictionary<string, NumberContainer> CommonEntityNumberProperties => commonEntityProperties.NumberProperties;
        public IDictionary<string, BooleanContainer> CommonEntityBooleanProperties => commonEntityProperties.BooleanProperties;

        public IOfflinePolicy OfflinePolicy => offlinePolicy;
        public UiConfiguration UiConfiguration => uiConfiguration;

        internal GameConfiguration(Dictionary<string, EntityDefinition> entities, HookConfigurationBuilder hooks, PropertyHolder globalProperties, PropertyHolder commonEntityProperties, 
            ISet<AchievementConfiguration> achievements, ISet<TutorialConfiguration> tutorials, IOfflinePolicy offlinePolicy, Dictionary<string, string> externalizedStrings, UiConfiguration uiConfiguration)
        {
            foreach(var entityConfiguration in entities)
            {
                this.entities.Add(entityConfiguration.Key, entityConfiguration.Value);
            }
            this.globalProperties = globalProperties;
            this.commonEntityProperties = commonEntityProperties;
            this.hooks = hooks.Build();
            this.globalProperties = globalProperties;
            this.tutorials = tutorials;
            foreach(var achievement in achievements)
            {
                this.achievements.Add(achievement.AchievementKey, achievement);
            }
            this.strings = externalizedStrings;
            this.offlinePolicy = offlinePolicy;
            this.uiConfiguration = uiConfiguration;
        }
    }
}