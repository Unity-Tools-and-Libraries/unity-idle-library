using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    public interface IHasResources
    {
        ResourceHolder GetResource(string id);
    }
}