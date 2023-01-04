using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.scripting.context;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public abstract class EntityBoughtEvent<T> : IScriptingContext where T : IBuyable
    {
        private T bought;
        public EntityBoughtEvent(T bought)
        {
            this.bought = bought;
        }
        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>()
            {
                { "bought", bought }
            };
        }
    }
}