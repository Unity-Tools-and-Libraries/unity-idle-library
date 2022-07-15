using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
using UnityEngine;

public abstract class TestsRequiringEngine
{
    protected IdleEngine engine;

    [SetUp]
    public void InitializeEngine()
    {
        engine = new IdleEngine();
    }
}
