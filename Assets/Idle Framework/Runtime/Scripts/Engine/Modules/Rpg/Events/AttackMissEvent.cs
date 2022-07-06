using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class AttackMissEvent : ScriptingContext
    {
        public const string EventName = "attack_missed";
        private Character attacker;
        private Character defender;

        public AttackMissEvent(Character attacker, Character defender)
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