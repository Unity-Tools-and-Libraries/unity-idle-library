﻿using io.github.thisisnozaku.idle.framework.Events;
using io.github.thisisnozaku.scripting.context;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Events
{
    /*
     * Base class for events involving a character modifier like an item, ability or status.
     */
    public abstract class RpgCharacterModifierEvent<T> : IScriptingContext where T : RpgCharacterModifier
    {
        public abstract Dictionary<string, object> GetContextVariables();
    }
}