using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public abstract class EntityEffectDefinition : EffectDefinition
    {
        /*
         * Calculate what the new value of the target property would be if this effect were applied.
         */
        public abstract BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine);
        public abstract IReadOnlyList<ModifiableProperty> GetAffectableProperties(IdleEngine engine);
    }
}