using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public interface ScriptingContext
    {
        Dictionary<string, object> GetScriptingProperties();
    }
}