using io.github.thisisnozaku.idle.framework;
using NUnit.Framework;
using UnityEngine;

public abstract class RequiresEngineTests
{
    protected IdleEngine engine;

    [SetUp]
    public void InitializeEngine()
    {
        engine = new IdleEngine(null);
    }
}
