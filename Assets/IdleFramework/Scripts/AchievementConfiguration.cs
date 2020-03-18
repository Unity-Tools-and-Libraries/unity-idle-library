namespace IdleFramework
{
    public class AchievementConfiguration : AchievementProperties
    {
        private readonly string achievementKey;
        private readonly StateMatcher gainedWhenMatcher;

        public AchievementConfiguration(string achivementKey, StateMatcher gainedWhenMatcher)
        {
            this.achievementKey = achivementKey;
            this.gainedWhenMatcher = gainedWhenMatcher;
        }

        public StateMatcher GainedWhenMatcher => gainedWhenMatcher;

        public string AchievementKey => achievementKey;
    }
}