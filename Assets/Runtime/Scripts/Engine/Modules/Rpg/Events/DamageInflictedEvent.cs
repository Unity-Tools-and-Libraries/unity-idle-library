using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public class DamageInflictedEvent : ScriptingContext
    {
        public const string EventName = "damage_inflicted";

        private RpgCharacter attacker;
        private BigDouble damage;
        private RpgCharacter defender;

        public DamageInflictedEvent(RpgCharacter attacker, BigDouble damage, RpgCharacter defender)
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