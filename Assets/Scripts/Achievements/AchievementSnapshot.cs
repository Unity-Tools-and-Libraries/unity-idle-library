using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Achievements
{
    public class AchievementSnapshot : Snapshot
    {
        private readonly string achievementKey;
        private readonly bool gained;

        public AchievementSnapshot(string achievementKey, bool gained)
        {
            this.achievementKey = achievementKey;
            this.gained = gained;
        }

        public string AchievementKey => achievementKey;

        public bool Gained => gained;
    }
}