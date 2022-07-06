using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class MissedByAttackEvent : ScriptingContext
    {
        public const string EventName = "missed_by_attack";
        private Character attacker;
        private Character defender;

        public MissedByAttackEvent(Character attacker, Character defender)
        {
            this.attacker = attacker;
            this.defender = defender;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender }
            };
        }
    }
}