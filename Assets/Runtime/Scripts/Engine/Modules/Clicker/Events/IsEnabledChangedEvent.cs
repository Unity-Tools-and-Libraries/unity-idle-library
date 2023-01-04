using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.scripting.context;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events
{
    public class IsEnabledChangedEvent : IScriptingContext
    {
        public const string EventName = "IsEnabledChanged";
        private IEnableable enableable;

        public IsEnabledChangedEvent(IEnableable enableable)
        {
            this.enableable = enableable;
        }

        public Dictionary<string, object> GetContextVariables()
        {
            return new Dictionary<string, object>()
            {
                { "enableable", enableable }
            };
        }
    }
}