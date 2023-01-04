using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.scripting.context;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public abstract class CombatInteractionEvent : IScriptingContext
    {
        public readonly RpgCharacter source;
        public readonly RpgCharacter target;

        public CombatInteractionEvent(RpgCharacter source, RpgCharacter target)
        {

        }

        public abstract Dictionary<string, object> GetContextVariables();
    }
}