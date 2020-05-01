using System;

namespace IdleFramework
{
    public class Achievement : AchievementProperties, CanSnapshot<AchievementSnapshot>
    {
        private readonly AchievementConfiguration configuration;
        private bool gained;

        public bool IsActive => gained;
        public string AchievementKey => configuration.AchievementKey;

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

        public AchievementSnapshot GetSnapshot(IdleEngine engine)
        {
            return new AchievementSnapshot(AchievementKey, gained);
        }

        public void LoadFromSnapshot(AchievementSnapshot snapshot)
        {
            if(AchievementKey != snapshot.AchievementKey)
            {
                throw new InvalidOperationException("Tried to load snapshot for wrong achievement");
            }
            gained = snapshot.Gained;
        }
    }
}