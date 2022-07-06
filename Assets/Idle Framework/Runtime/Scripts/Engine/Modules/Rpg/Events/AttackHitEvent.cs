using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class AttackHitEvent : ScriptingContext
    {
        public const string EventName = "attack_hit";

        private Character attacker;
        private Character defender;
        private BigDouble attackDamage;

        public AttackHitEvent(Character attacker, Character defender, BigDouble attackDamage)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.attackDamage = attackDamage;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender },
                { "damage", attackDamage }
            };
        }
    }
}