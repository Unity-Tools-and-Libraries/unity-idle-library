using BreakInfinity;

using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * An abstract base class for something that applies a change to a character.
     */
    public abstract class RpgCharacterModifier : EntityModifier<RpgCharacter>
    {
        public Dictionary<string, List<string>> EventTriggers;
        public string Description;
        public RpgCharacterModifier(IdleEngine engine, long id, string description, Dictionary<string, Tuple<string, string>> modifications, Dictionary<string, List<string>> events = null) : base(engine, id, modifications)
        {
            this.Description = description;
            this.EventTriggers = events;
        }
    }
}