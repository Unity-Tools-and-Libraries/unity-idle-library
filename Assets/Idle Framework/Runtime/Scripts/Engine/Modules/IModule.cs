using io.github.thisisnozaku.idle.framework.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules
{
    public interface IModule
    {
        IDictionary<string, IDictionary<string, IDefinition>> GetDefinitions();
        void SetEngineProperties(IdleEngine engine);
    }
}