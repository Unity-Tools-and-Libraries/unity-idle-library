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
        private Dictionary<string, ValueContainer> globalProperties = new Dictionary<string, ValueContainer>();
        private Dictionary<string, GlobalSingletonPropertyDefinition> globalSingletonProperties = new Dictionary<string, GlobalSingletonPropertyDefinition>();
        private ISet<ModifierDefinitionProperties> modifiers = new HashSet<ModifierDefinitionProperties>();
        private Dictionary<string, ValueContainer> sharedCustomEntityProperties = new Dictionary<string, ValueContainer>();
        private HookConfigurationBuilder hooks = new HookConfigurationBuilder();
        private ISet<SingletonEntityDefinition> singletons = new HashSet<SingletonEntityDefinition>();
        private ISet<AchievementConfiguration> achievements = new HashSet<AchievementConfiguration>();
        private ISet<TutorialConfiguration> tutorials = new HashSet<TutorialConfiguration>();
        private Dictionary<string, string> externalizedStrings = new Dictionary<string, string>();
        /**
         * Add a definition for a new entity.
         */
        public GameConfigurationBuilder WithEntity(EntityDefinitionBuilder definition)
        {
            var entity = new EntityDefinition(definition);
            entities.Add(entity);
            return this;
        }

        /*
         * Add a hook that is called whenever update occurs.
         */
        public void WithUpdateHook(Action<IdleEngine, float> hook)
        {
            hooks.AddUpdateHook(hook);
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

        public GameConfigurationBuilder WithEventHook(string eventName, Action<IdleEngine, object> hook)
        {
            hooks.AddEventHook(eventName, hook);
            return this;
        }

        public GameConfigurationBuilder WithTutorial(TutorialConfigurationBuilder.TutorialConfigurationTerminal tutorialConfigurationBuilder)
        {
            tutorials.Add(tutorialConfigurationBuilder.Build());
            return this;
        }

        public GameConfigurationBuilder WithStartupHook(Action<IdleEngine> hook)
        {
            hooks.AddStartHook(hook);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, ValueContainer propertyValue)
        {
            globalProperties.Add(propertyName, propertyValue);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName)
        {
            globalProperties.Add(propertyName, NullValue.INSTANCE);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, string singletonType)
        {
            globalSingletonProperties.Add(propertyName, new GlobalSingletonPropertyDefinition(singletonType));
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, string singletonType, string defaultValue)
        {
            globalSingletonProperties.Add(propertyName, new GlobalSingletonPropertyDefinition(singletonType, defaultValue));
            return this;
        }

        public GameConfigurationBuilder WithCustomEntityProperty(string propertyName, ValueContainer value)
        {
            sharedCustomEntityProperties.Add(propertyName, value);
            return this;
        }

        public void WithExternalizedString(object p)
        {
            throw new NotImplementedException();
        }

        public GameConfiguration Build()
        {
            return new GameConfiguration(entities, 
                modifiers, 
                hooks, 
                singletons, 
                sharedCustomEntityProperties, 
                globalProperties,
                globalSingletonProperties,
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