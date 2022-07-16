using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine;
using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestsRequiringEngine
{
    protected IdleEngine engine;

    [SetUp]
    public void InitializeEngine()
    {
        engine = new IdleEngine();
        UserData.RegisterType<TestEntity>();
        UserData.RegisterType<TestModifier>();
    }

    public class TestEntity : Entity
    {
        public BigDouble foo;
        public BigDouble Bar { get; set; } = 1;
        public TestEntity(IdleEngine engine, long id, BigDouble foo = default(BigDouble)) : base(engine, id)
        {
            this.foo = foo;
        }
    }

    public class TestModifier : EntityModifier<TestEntity>
    {
        public TestModifier(IdleEngine engine, long id, Dictionary<string, Tuple<string, string>> modifications) : base(engine, id, modifications)
        {
        }
    }
}
