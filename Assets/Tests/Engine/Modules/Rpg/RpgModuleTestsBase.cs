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
        rpgModule = new RpgModule();
        random = new RiggedRandom();

        engine.OverrideRandomNumberGenerator(random);
    }

    public void Configure()
    {
        rpgModule.AddCreature(new CreatureDefinition.Builder().Build(1));
        rpgModule.AddCreature(new CreatureDefinition.Builder().WithProperty("foo", "bar").Build(2));
        rpgModule.AddEncounter(new EncounterDefinition(1, Tuple.Create(1L, 0)));

        engine.AddModule(rpgModule);
    }

    public class RiggedRandom : System.Random
    {
        private Queue<double> nextValues = new Queue<double>();
        private int callCount = 0;

        public void SetNextValues(params double[] values)
        {
            foreach (var next in values)
            {
                nextValues.Enqueue(next);
            }
        }

        public override int Next()
        {
            callCount++;
            return (int)nextValues.Dequeue();
        }

        public override int Next(int maxValue)
        {
            callCount++;
            int next = (int)nextValues.Dequeue();
            if(next >= maxValue)
            {
                throw new InvalidOperationException(String.Format("Tried to return a value less than {0} but next forced value was {1} on call {2}", maxValue, next, callCount));
            }
            return next;
        }
    }
}
