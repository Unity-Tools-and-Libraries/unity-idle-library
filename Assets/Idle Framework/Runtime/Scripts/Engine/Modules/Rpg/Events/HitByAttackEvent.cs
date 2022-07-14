using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class HitByAttackEvent : AttackEvent, ScriptingContext
    {
        public const string EventName = "hit_by_attack";

        public HitByAttackEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }
}