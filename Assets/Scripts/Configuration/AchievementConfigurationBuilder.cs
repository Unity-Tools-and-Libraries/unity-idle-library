namespace IdleFramework.Configuration
{
    public class AchievementConfigurationBuilder
    {
        private readonly string achievementKey;
        private StateMatcher gainedWhenMatcher;

        public AchievementConfigurationBuilder(string achievementKey)
        {
            this.achievementKey = achievementKey;
        }

        public string AchievementKey => achievementKey;
        public StateMatcher GainedWhenMatcher => gainedWhenMatcher;

        public AchievementConfigurationBuilder GainedWhen(StateMatcher matcher)
        {
            gainedWhenMatcher = matcher;
            return this;
        }
    }
}