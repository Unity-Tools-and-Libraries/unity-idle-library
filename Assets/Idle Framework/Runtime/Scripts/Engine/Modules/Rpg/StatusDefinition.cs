using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * A status is a temporary/removable effect that can be applied to a character, for both positive effects (like a buff from an ability) and negative (such as various debuffs like poision or blindness, etc.). 
     */
    public class StatusDefinition : CharacterModifierDefinition
    {
        private StatusDefinition(string id, string description, IDictionary<string, string> Modifications, IDictionary<string, object> properties = null, IDictionary<string, List<string>> Events = null) : base(id, description, properties, Modifications, Events)
        {
        }

        public class Builder : CharacterModifierDefinition.Builder<StatusDefinition>
        {
            public override StatusDefinition Build(string id, string description)
            {
                return new StatusDefinition(id, description, Modifications);
            }
        }
    }
}