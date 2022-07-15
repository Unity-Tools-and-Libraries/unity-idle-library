using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * An ability is an always-available power or special trait that a character has, from a dragon's ability to breath fire, a ghost's intangibility, the ability of a superhero to fly, etc.
     */
    public class AbilityDefinition: EntityModifier<RpgCharacter>
    {
        public AbilityDefinition(long id, IdleEngine engine, string description, Dictionary<string, Tuple<string, string>> modifications, IDictionary<string, List<string>> events = null) : base(engine, id, modifications)
        {
        }
    }
}