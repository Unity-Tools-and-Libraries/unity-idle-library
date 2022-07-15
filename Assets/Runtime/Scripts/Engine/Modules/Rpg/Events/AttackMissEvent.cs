using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class AttackMissEvent : AttackEvent
    {
        public const string EventName = "attack_missed";

        public AttackMissEvent(RpgCharacter attacker, RpgCharacter defender, BigDouble attackDamage) : base(attacker, defender, attackDamage)
        {
        }
    }
}