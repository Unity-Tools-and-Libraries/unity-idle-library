using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using io.github.thisisnozaku.scripting.context;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    public class CharacterResurrectedEvent : IScriptingContext
    {
        public const string EventName = "resurrected";
        public RpgCharacter resurrected { get; }
        private Dictionary<string, object> variables;

        public CharacterResurrectedEvent(RpgCharacter resurrected)
        {
            this.resurrected = resurrected;
            variables = new Dictionary<string, object>()
            {
                { "resurrected", resurrected }
            };
        }

        public Dictionary<string, object> GetContextVariables()
        {
            return variables;
        }
    }
}