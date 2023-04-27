using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using UnityEngine;

public class RpgPlayer : Player
{

    public RpgPlayer(IdleEngine engine, long id) : base(engine, id)
    {
        var fields = GetType().GetFields();
        foreach(var field in fields)
        {
            this.traversableFields.Add(field);
        }
    }

    public RpgCharacter Character;
}
