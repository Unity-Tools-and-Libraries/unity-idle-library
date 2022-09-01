using BreakInfinity;
using io.github.thisisnozaku.scripting.context;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class StageChangedEvent : IScriptingContext
    {
        public const string EventName = "stageChanged";
        public readonly BigDouble stage;

        public StageChangedEvent(BigDouble stage)
        {
            this.stage = stage;
        }

        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>()
            {
                { "stage", stage }
            };
        }
    }
}