using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public abstract class EntityBoughtEvent<T> : ScriptingContext where T : IBuyable
    {
        private T bought;
        public EntityBoughtEvent(T bought)
        {
            this.bought = bought;
        }
        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "bought", bought }
            };
        }
    }
}