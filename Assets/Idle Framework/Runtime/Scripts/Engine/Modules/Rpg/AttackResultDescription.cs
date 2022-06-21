using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class AttackResultDescription
    {
        public readonly bool AttackHit;

        public AttackResultDescription(bool attackHit)
        {
            AttackHit = attackHit;
        }
    }
}