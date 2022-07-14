
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgItem : EntityModifier<RpgCharacter>
    {
        public RpgItem(long id, IdleEngine engine, string[] usedSlots, Dictionary<string, Tuple<string, string>> effects) : base(engine, id, effects)
        {
            this.UsedSlots = usedSlots;
        }

        /*
         * The slots used by this item.
         */
        public readonly string[] UsedSlots;

        public override void Apply(RpgCharacter target)
        {
            throw new NotImplementedException();
        }

        public override void Unapply(RpgCharacter target)
        {
            throw new NotImplementedException();
        }
    }
}