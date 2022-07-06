using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CharacterActedEvent : ScriptingContext
    {
        public const string EventName = "character_acted";

        private Character character;

        public CharacterActedEvent(Character character)
        {
            this.character = character;
        }

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            return new Dictionary<string, object>()
            {
                { "character", character }
            };
        }
    }
}