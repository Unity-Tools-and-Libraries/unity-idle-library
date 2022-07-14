using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RpgModuleTestsBase : TestsRequiringEngine
{
    protected RpgModule rpgModule;
    protected RiggedRandom random;
    
    [SetUp]
    public void Setup()
    {
        base.InitializeEngine();
        rpgModule = new RpgModule();
        random = new RiggedRandom();

        engine.OverrideRandomNumberGenerator(random);
    }

    public void Configure()
    {
        rpgModule.AddCreature(new CreatureDefinition.Builder().Build(1));
        rpgModule.AddEncounter(new EncounterDefinition(1, Tuple.Create(1L, 0)));

        engine.AddModule(rpgModule);
    }

    public class RiggedRandom : System.Random
    {
        private Queue<double> nextValues = new Queue<double>();

        public void SetNextValues(params double[] values)
        {
            foreach (var next in values)
            {
                nextValues.Enqueue(next);
            }
        }

        public override int Next()
        {
            return (int)nextValues.Dequeue();
        }

        public override int Next(int maxValue)
        {
            return (int)nextValues.Dequeue() % Math.Max(maxValue, 1) ;
        }
    }
}
