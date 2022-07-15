using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class DamageTakenEvent : ScriptingContext
    {
        public const string EventName = "damage_taken";

        private RpgCharacter attacker;
        private BigDouble damage;
        private RpgCharacter defender;

        public DamageTakenEvent(RpgCharacter attacker, BigDouble damage, RpgCharacter defender)
        {
            this.attacker = attacker;
            this.damage = damage;
            this.defender = defender;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "attacker", attacker },
                {"defender", defender },
                { "damage", damage }
            };
        }
    }
}