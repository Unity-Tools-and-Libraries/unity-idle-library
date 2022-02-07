using IdleFramework.Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RequiresEngineTests
{
    protected IdleFramework.IdleEngine engine;

    public void InitializeEngine(EngineConfiguration configuration = null, GameObject go = null)
    {
        engine = new IdleFramework.IdleEngine(configuration, go);
    }
}
