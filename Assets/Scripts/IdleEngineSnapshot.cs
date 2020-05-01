using IdleFramework.Achievements;
using IdleFramework.Entities;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class IdleEngineSnapshot : Snapshot
    {
        private readonly ICollection<EntitySnapshot> entities;
        private readonly ICollection<AchievementSnapshot> achievements;
        private readonly PropertyHolderSnapshot globalProperties;
        private readonly DateTime timeSinceLastUpdate;

        public IdleEngineSnapshot(ICollection<EntitySnapshot> entities, ICollection<AchievementSnapshot> achievements, PropertyHolderSnapshot globalProperties, DateTime timeSinceLastUpdate)
        {
            this.entities = entities;
            this.achievements = achievements;
            this.globalProperties = globalProperties;
            this.timeSinceLastUpdate = timeSinceLastUpdate;
        }

        public IEnumerable<EntitySnapshot> Entities => entities;

        public IEnumerable<AchievementSnapshot> Achievements => achievements;

        public PropertyHolderSnapshot GlobalProperties => globalProperties;

        public DateTime TimeSinceLastUpdate => timeSinceLastUpdate;
    }
}