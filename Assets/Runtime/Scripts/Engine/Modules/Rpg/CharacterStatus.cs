
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * A status is a temporary/removable effect that can be applied to a character, for both positive effects (like a buff from an ability) and negative (such as various debuffs like poision or blindness, etc.). 
     */
    public class CharacterStatus : RpgCharacterModifier
    {
        protected CharacterStatus(IdleEngine engine, long id, string description, Dictionary<string, Tuple<string, string>> Modifications, Dictionary<string, List<string>> events) : base(engine, id, Modifications, events)
        {
            
        }

        public class Builder : Builder<CharacterStatus>
        {
            private Dictionary<string, List<string>> events = new Dictionary<string, List<string>>();
            public override CharacterStatus Build(IdleEngine engine, long id)
            {
                return new CharacterStatus(engine, id, "", modifications, events);
            }

            public Builder WithEventTrigger(string trigger, string effect)
            {
                List<string> triggers;
                if(!events.TryGetValue(trigger, out triggers))
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