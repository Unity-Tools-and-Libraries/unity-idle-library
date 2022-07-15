
using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class CharacterDamagedEvent : ScriptingContext
    {
        public const string EventName = "character_damaged";
        private RpgCharacter damagedCharacter;
        private BigDouble damageInflicted;
        private RpgCharacter attacker;

        public CharacterDamagedEvent(RpgCharacter damagedCharacter, BigDouble damageInflicted, RpgCharacter attacker)
        {
            this.damagedCharacter = damagedCharacter;
            this.damageInflicted = damageInflicted;
            this.attacker = attacker;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
        {
            { "attacker", attacker },
            { "damage", damageInflicted },
            { "defender", damagedCharacter }
        };
        }
    }
}