
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules
{
    public interface IModule
    {
        void ConfigureEngine(IdleEngine engine);
        void AssertReady();
    }
}