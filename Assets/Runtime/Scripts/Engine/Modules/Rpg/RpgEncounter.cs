using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgEncounter : Entity
    {
        public RpgEncounter(IdleEngine engine, long id, BigDouble level) : base(engine, id)
        {
            this.Level = level;
        }

        public BigDouble Level { get; }
        public bool IsActive { get; set; }
        [TraversableFieldOrProperty]
        public IList<RpgCharacter> Creatures { get; internal set; } = new List<RpgCharacter>();
    }
}