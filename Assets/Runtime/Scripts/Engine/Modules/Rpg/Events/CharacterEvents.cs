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