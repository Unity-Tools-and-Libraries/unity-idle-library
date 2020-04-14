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
        /*
         * Definitions for the entities that exist in the game.
         */
        private Dictionary<string, EntityDefinition> entities = new Dictionary<string, EntityDefinition>();
        /*
         * Global properties for the game.
         */
        private readonly PropertyHolder globalProperties = new PropertyHolder();
        private readonly PropertyHolder commonEntityProperties = new PropertyHolder();
        private HookConfigurationBuilder hooks = new HookConfigurationBuilder();
        private ISet<AchievementConfiguration> achievements = new HashSet<AchievementConfiguration>();
        private ISet<TutorialConfiguration> tutorials = new HashSet<TutorialConfiguration>();
        private Dictionary<string, string> externalizedStrings = new Dictionary<string, string>();
        /**
         * Add a definition for a new entity.
         */
        public GameConfigurationBuilder WithEntity(EntityDefinitionBuilder definition)
        {
            var entity = new EntityDefinition(definition);
            entities.Add(entity.EntityKey, entity);
            return this;
        }

        /*
         * Add a hook that is called whenever update occurs.
         */
        public GameConfigurationBuilder WithUpdateHook(Action<IdleEngine, float> hook)
        {
            hooks.AddUpdateHook(hook);
            return this;
        }

        public GameConfigurationBuilder WithBeforeUpdateHook(Action<IdleEngine, float> hook)
        {
            hooks.AddBeforeUpdateHook(hook);
            return this;
        }

        public GameConfigurationBuilder WithAchievement(AchievementConfigurationBuilder achievement)
        {
            achievements.Add(new AchievementConfiguration(achievement));
            return this;
        }

        internal GameConfigurationBuilder WithSingletonEntity(EntityDefinitionBuilder entity)
        {
            entity.AsSingleton();
            return WithEntity(entity);
        }

        public GameConfigurationBuilder WithEventHook(string eventName, Action<IdleEngine, object> hook)
        {
            hooks.AddEventHook(eventName, hook);
            return this;
        }

        public GameConfigurationBuilder WithTutorial(TutorialConfigurationBuilder tutorialConfigurationBuilder)
        {
            tutorials.Add(tutorialConfigurationBuilder.Build());
            return this;
        }

        public GameConfigurationBuilder WithStartupHook(Action<IdleEngine> hook)
        {
            hooks.AddStartHook(hook);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, BooleanContainer propertyValue)
        {
            globalProperties.Set(propertyName, propertyValue);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, NumberContainer propertyValue)
        {
            globalProperties.Set(propertyName, propertyValue);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, StringContainer propertyValue)
        {
            globalProperties.Set(propertyName, propertyValue);
            return this;
        }

        public void WithExternalizedString(object p)
        {
            throw new NotImplementedException();
        }

        public GameConfiguration Build()
        {
            return new GameConfiguration(entities, 
                hooks, 
                globalProperties,
                commonEntityProperties,
                achievements, 
                tutorials,
                externalizedStrings);
        }
    }


    public class GlobalSingletonPropertyDefinition
    {
        private readonly string type;
        private readonly string defaultInstance;

        public GlobalSingletonPropertyDefinition(string type) : this(type, null)
        {

        }

        public GlobalSingletonPropertyDefinition(string type, string defaultInstance)
        {
            this.type = type;
            this.defaultInstance = defaultInstance;
        }

        public string Type => type;

        public string DefaultInstance => defaultInstance;
    }

}