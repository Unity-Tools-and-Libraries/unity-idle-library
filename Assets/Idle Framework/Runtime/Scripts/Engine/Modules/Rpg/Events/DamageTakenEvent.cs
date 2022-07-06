using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class DamageTakenEvent : ScriptingContext
    {
        public const string EventName = "damage_taken";

        private Character attacker;
        private BigDouble damage;
        private Character defender;

        public DamageTakenEvent(Character attacker, BigDouble damage, Character defender)
        {
            this.attacker = attacker;
            this.damage = damage;
            this.defender = defender;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
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