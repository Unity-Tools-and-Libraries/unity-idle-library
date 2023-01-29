
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CharacterItem : RpgCharacterModifier
    {
        public CharacterItem(long id, IdleEngine engine, string description, string[] usedSlots, Dictionary<string, Tuple<string, string>> modifications, Dictionary<string, List<String>> events) : base(engine, id, description, modifications, events)
        {
            this.UsedSlots = usedSlots;
        }

        /*
         * The slots used by this item.
         */
        public readonly string[] UsedSlots;

        public class Builder : EntityModifier<RpgCharacter>.Builder<CharacterItem>
        {
            private List<string> usedSlots = new List<string>();
            private Dictionary<string, List<string>> events = new Dictionary<string, List<string>>();
            private string description;
            public override CharacterItem Build(IdleEngine engine, long id)
            {
                return new CharacterItem(id, engine, description, usedSlots.ToArray(), modifications, events);
            }

            public Builder WithDescription(string description)
            {
                this.description = description;
                return this;
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