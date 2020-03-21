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
        private ISet<ModifierDefinitionProperties> modifiers = new HashSet<ModifierDefinitionProperties>();
        private Dictionary<string, ValueContainer> sharedCustomEntityProperties = new Dictionary<string, ValueContainer>();
        private HookConfigurationBuilder hooks = new HookConfigurationBuilder();
        private ISet<SingletonEntityDefinition> singletons = new HashSet<SingletonEntityDefinition>();
        private ISet<AchievementConfiguration> achievements = new HashSet<AchievementConfiguration>();
        private ISet<TutorialConfiguration> tutorials = new HashSet<TutorialConfiguration>();
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

        public GameConfigurationBuilder WithTutorial(TutorialConfigurationBuilder.TerminalTutorialConfigurationBuilderStage tutorialConfigurationBuilder)
        {
            tutorials.Add(tutorialConfigurationBuilder.Build());
            return this;
        }

        public GameConfigurationBuilder WithStartupHook(EngineStartHook hook)
        {
            hooks.AddStartHook(hook);
            return this;
        }

        public GameConfigurationBuilder WithCustomGlobalProperty(string propertyName, ValueContainer propertyValue)
        {
            globalProperties.Add(propertyName, propertyValue);
            return this;
        }

        public GameConfigurationBuilder WithCustomEntityProperty(string propertyName, ValueContainer value)
        {
            sharedCustomEntityProperties.Add(propertyName, value);
            return this;
        }

        public GameConfiguration Build()
        {
            return new GameConfiguration(entities, modifiers, hooks, singletons, sharedCustomEntityProperties, globalProperties, achievements, tutorials);
        }
    }
}