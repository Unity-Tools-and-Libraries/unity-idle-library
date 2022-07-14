using System;
using System.Linq;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Persistence
{
    public class EngineSnapshot
    {
        public readonly Dictionary<string, object> Properties;
        public readonly Dictionary<string, Dictionary<string, string>> Listeners;

        public EngineSnapshot(Dictionary<string, object> properties, Dictionary<string, Dictionary<string, string>> listeners)
        {
            this.Properties = properties;
            this.Listeners = listeners;
        }
    }
}