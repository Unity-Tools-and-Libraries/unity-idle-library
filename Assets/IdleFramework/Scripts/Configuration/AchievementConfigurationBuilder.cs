namespace IdleFramework
{
    public class AchievementConfigurationBuilder: Builder<AchievementConfiguration>
    {
        private readonly string achievementKey;
        private StateMatcher gainedWhenMatcher;

        public AchievementConfigurationBuilder(string achievementKey)
        {
            this.achievementKey = achievementKey;
        }

        public string AchievementKey => achievementKey;
        public StateMatcher GainedWhenMatcher => gainedWhenMatcher;

        public AchievementConfiguration Build()
        {
            return new AchievementConfiguration(achievementKey, gainedWhenMatcher);
        }

        public AchievementConfigurationBuilder GainedWhen(StateMatcher matcher)
        {
            gainedWhenMatcher = matcher;
            return this;
        }
    }
}