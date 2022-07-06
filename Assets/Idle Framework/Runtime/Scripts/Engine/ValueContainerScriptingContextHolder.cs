using io.github.thisisnozaku.idle.framework.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerScriptingContextHolder : ScriptingContext
{
    private IdleEngine engine;
    private string path;
    public ContainerScriptingContextHolder(IdleEngine engine, string path)
    {
        this.engine = engine;
        this.path = path;
    }
    public Dictionary<string, object> GetScriptingContext(string contextType = null)
    {
        return engine.GetProperty(path).GetScriptingContext();
    }
}
