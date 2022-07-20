using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class StageChangedEvent : ScriptingContext
    {
        public const string EventName = "stageChanged";
        public readonly BigDouble stage;

        public StageChangedEvent(BigDouble stage)
        {
            this.stage = stage;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "stage", stage }
            };
        }
    }
}