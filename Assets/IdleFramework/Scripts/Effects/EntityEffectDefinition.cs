using BreakInfinity;
using System;

namespace IdleFramework
{
    public abstract class EntityEffectDefinition : EffectDefinition
    {
        /*
         * Calculate what the new value of the target property would be if this effect were applied.
         */
        public abstract BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine);
    }
}