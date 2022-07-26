using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class Timer : IUpdateable
    {
        public readonly double Duration;
        private double remainingTime;
        public readonly string Handler;
        public bool Triggered { get; private set; }

        public Timer(double duration, string handler)
        {
            Duration = duration;
            this.remainingTime = duration;
            Handler = handler;
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            this.remainingTime = Math.Max(0, remainingTime - (double)deltaTime);
            if(remainingTime <= 0f)
            {
                engine.Scripting.EvaluateStringAsScript(Handler);
                Triggered = true;
            }
        }
    }
}
