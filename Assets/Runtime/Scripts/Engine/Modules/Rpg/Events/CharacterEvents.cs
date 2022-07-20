using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CharacterActedEvent : ScriptingContext
    {
        public const string EventName = "character_acted";

        private RpgCharacter character;

        public CharacterActedEvent(RpgCharacter character)
        {
            this.character = character;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "character", character }
            };
        }
    }

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

    public class CharacterDiedEvent : ScriptingContext
    {
        public const string EventName = "character_died";
        private RpgCharacter died;
        private RpgCharacter killer;
        public CharacterDiedEvent(RpgCharacter died, RpgCharacter killer = null)
        {
            this.died = died;
            this.killer = killer;
        }

        public Dictionary<string, object> GetScriptingProperties()
        {
            return new Dictionary<string, object>()
            {
                { "died", died },
                { "killer", killer }
            };
        }
    }
}