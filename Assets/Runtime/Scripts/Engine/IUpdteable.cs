using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine;
using UnityEngine;

public interface IUpdateable
{
    void Update(IdleEngine engine, float deltaTime);
}