
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgItem : RpgCharacterModifier
    {
        public RpgItem(long id, IdleEngine engine, string[] usedSlots, Dictionary<string, Tuple<string, string>> modifications, Dictionary<string, List<String>> events) : base(engine, id, modifications, events)
        {
            this.UsedSlots = usedSlots;
        }

        /*
         * The slots used by this item.
         */
        public readonly string[] UsedSlots;

        public class Builder : EntityModifier<RpgCharacter>.Builder<RpgItem>
        {
            private List<string> usedSlots = new List<string>();
            private Dictionary<string, List<string>> events = new Dictionary<string, List<string>>();
            public override RpgItem Build(IdleEngine engine, long id)
            {
                return new RpgItem(id, engine, usedSlots.ToArray(), modifications, events);
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