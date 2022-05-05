using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Configuration;
using UnityEngine;

public abstract class RequiresEngineTests
{
    protected IdleEngine engine;

    public void InitializeEngine(EngineConfiguration configuration = null, GameObject go = null)
    {
        engine = new IdleEngine(configuration, go);
    }
}
