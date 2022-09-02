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

    [TearDown]
    public void Teardow()
    {
        Script.GlobalOptions.CustomConverters.Clear();
    }

    public class TestEntity : Entity
    {
        [TraversableFieldOrProperty]
        public BigDouble foo;
        [TraversableFieldOrProperty]
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
