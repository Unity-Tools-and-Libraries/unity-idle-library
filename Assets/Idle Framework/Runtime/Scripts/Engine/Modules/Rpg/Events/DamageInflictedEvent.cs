using BreakInfinity;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class DamageInflictedEvent : ScriptingContext
    {
        public const string EventName = "damage_inflicted";

        private Character attacker;
        private BigDouble damage;
        private Character defender;

        public DamageInflictedEvent(Character attacker, BigDouble damage, Character defender)
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