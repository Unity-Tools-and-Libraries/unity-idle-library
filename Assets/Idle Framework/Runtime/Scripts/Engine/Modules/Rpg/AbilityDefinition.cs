using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * An ability is an always-available power or special trait that a character has, from a dragon's ability to breath fire, a ghost's intangibility, the ability of a superhero to fly, etc.
     */
    public class AbilityDefinition: CharacterModifierDefinition
    {
        private Dictionary<string, object> properties;

        public AbilityDefinition(string id, string description, IDictionary<string, string> modifications, IDictionary<string, object> properties = null, IDictionary<string, List<string>> events = null) : base(id, description, properties, modifications, events)
        {
        }
    }
}