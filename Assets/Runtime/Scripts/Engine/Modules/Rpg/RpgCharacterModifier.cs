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
        public RpgCharacterModifier(long id, IdleEngine engine, Dictionary<string, Tuple<string, string>> modifications, Dictionary<string, Tuple<string, string>> events = null) : base(engine, id, modifications)
        {
            
        }
    }
}