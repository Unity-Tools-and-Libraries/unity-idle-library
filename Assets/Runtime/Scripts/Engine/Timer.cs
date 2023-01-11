using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class Timer : IUpdateable
    {
        public readonly double Duration;
        public readonly string Description;
        private double remainingTime;
        public readonly string Handler;
        public readonly bool Repeat;
        public bool Triggered { get; private set; }

        public Timer(double duration, string handler, string description = "", bool repeat = false)
        {
            Duration = duration;
            this.remainingTime = duration;
            this.Description = description;
            Handler = handler;
            this.Repeat = repeat;
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
