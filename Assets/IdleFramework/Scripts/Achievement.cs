using System;

namespace IdleFramework
{
    public class Achievement : AchievementProperties
    {
        private readonly AchievementConfiguration configuration;
        private bool gained;

        public bool IsActive => gained;
        public string AchievementKey => ((AchievementProperties)configuration).AchievementKey;

        public Achievement(AchievementConfiguration configuration)
        {
            this.configuration = configuration;
            this.gained = false;
        }

        internal void Gain()
        {
            this.gained = true;
        }

        internal bool ShouldBeActive(IdleEngine idleEngine)
        {
            return configuration.GainedWhenMatcher.Matches(idleEngine);
        }
    }
}