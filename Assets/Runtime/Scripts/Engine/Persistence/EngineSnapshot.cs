using System;
using System.Linq;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Achievements;

namespace io.github.thisisnozaku.idle.framework.Engine.Persistence
{
    public class EngineSnapshot
    {
        public readonly Dictionary<string, object> Properties;
        public readonly Dictionary<string, Dictionary<string, string>> Listeners;
        public readonly List<Achievement> Achievements;

        public EngineSnapshot(Dictionary<string, object> properties, List<Achievement> achievements, Dictionary<string, Dictionary<string, string>> listeners)
        {
            this.Properties = properties;
            this.Listeners = listeners;
            this.Achievements = achievements;
        }
    }
}