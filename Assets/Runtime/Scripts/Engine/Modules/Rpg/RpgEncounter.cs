using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgEncounter : Entity
    {
        public RpgEncounter(IdleEngine engine, double id, BigDouble level) : base(engine, id)
        {
            this.Level = level;
        }

        public BigDouble Level { get; }
        public bool IsActive => Creatures.Any(c => c.IsAlive) && this.Engine.GetPlayer<RpgPlayer>().Character.IsAlive;
        [TraversableFieldOrProperty]
        public IList<RpgCharacter> Creatures { get; internal set; } = new List<RpgCharacter>();
    }
}