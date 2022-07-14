using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class RpgEncounter : Entity
    {
        public RpgEncounter(IdleEngine engine) : base(engine)
        {
        }

        public bool IsActive { get; internal set; }
        public IList<RpgCharacter> Creatures { get; internal set; } = new List<RpgCharacter>();
    }
}