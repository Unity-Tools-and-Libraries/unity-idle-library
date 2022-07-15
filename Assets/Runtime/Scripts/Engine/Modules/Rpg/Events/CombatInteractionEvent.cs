using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public abstract class CombatInteractionEvent : ScriptingContext
    {
        public readonly RpgCharacter source;
        public readonly RpgCharacter target;

        public CombatInteractionEvent(RpgCharacter source, RpgCharacter target)
        {

        }

        public abstract Dictionary<string, object> GetScriptingProperties();
    }
}