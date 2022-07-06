using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ScriptingContext
{
    Dictionary<string, object> GetScriptingContext(string contextType = null);
}
