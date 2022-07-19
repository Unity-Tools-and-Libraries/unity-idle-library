using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * An ability is an always-available power or special trait that a character has, from a dragon's ability to breath fire, a ghost's intangibility, the ability of a superhero to fly, etc.
     */
    public class CharacterAbility: RpgCharacterModifier
    {
        protected CharacterAbility(IdleEngine engine, long id, string description, Dictionary<string, Tuple<string, string>> modifications, Dictionary<string, List<string>> eventTriggers = null) : base(engine, id, modifications, eventTriggers)
        {
            
        }

        public class Builder : Builder<CharacterAbility>
        {
            private Dictionary<string, List<string>> events = new Dictionary<string, List<string>>();
            public override CharacterAbility Build(IdleEngine engine, long id)
            {
                return new CharacterAbility(engine, id, "", modifications, events);
            }

            public Builder WithEventTrigger(string trigger, string effect)
            {
                List<string> triggers;
                if (!events.TryGetValue(trigger, out triggers))
                {
                    triggers = new List<string>();
                    events[trigger] = triggers;
                }
                triggers.Add(effect);
                return this;
            }
        }
    }
}