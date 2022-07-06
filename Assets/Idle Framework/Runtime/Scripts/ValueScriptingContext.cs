using io.github.thisisnozaku.idle.framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueScriptingContext : ScriptingContext
{
    private object value;
    public ValueScriptingContext(object value)
    {
        this.value = ValueContainer.NormalizeValue(value);
    }
    public Dictionary<string, object> GetScriptingContext(string contextType = null)
    {
        return new Dictionary<string, object>()
        {
            { "value", value }
        };
    }
}
