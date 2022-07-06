using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events
{
    public class UpgradeBoughtEvent : ScriptingContext
    {
        private string upgradeId;

        public UpgradeBoughtEvent(string upgradeId)
        {
            this.upgradeId = upgradeId;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {
                { "upgrade", upgradeId }
            };
        }
    }
}