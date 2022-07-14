using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class AttackHitEvent : AttackEvent, ScriptingContext
    {
        public const string EventName = "attack_hit";

        public AttackHitEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }
}