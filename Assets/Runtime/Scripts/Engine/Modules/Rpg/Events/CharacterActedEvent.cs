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
}