
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    /*
     * A status is a temporary/removable effect that can be applied to a character, for both positive effects (like a buff from an ability) and negative (such as various debuffs like poision or blindness, etc.). 
     */
    public class CharacterStatus : EntityModifier<RpgCharacter>
    {
        private CharacterStatus(IdleEngine engine, long id, Dictionary<string, Tuple<string, string>> Modifications) : base(engine, id, Modifications)
        {
            
        }

        public class Builder : EntityModifier<RpgCharacter>.Builder<CharacterStatus>
        {
            public override CharacterStatus Build(IdleEngine engine, long id)
            {
                return new CharacterStatus(engine, id, modifications);
            }
        }
    }
}