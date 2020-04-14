namespace IdleFramework
{
    public class AchievementConfiguration : AchievementProperties
    {
        private readonly string achievementKey;
        private readonly StateMatcher gainedWhenMatcher;

        public AchievementConfiguration(AchievementConfigurationBuilder builder)
        {
            achievementKey = builder.AchievementKey;
            gainedWhenMatcher = builder.GainedWhenMatcher;
        }

        public StateMatcher GainedWhenMatcher => gainedWhenMatcher;

        public string AchievementKey => achievementKey;
    }
}