using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
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