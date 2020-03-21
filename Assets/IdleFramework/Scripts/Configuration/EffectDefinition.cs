using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{    
    public interface EffectDefinition
    {
        object CalculateEffect(ModifiableProperty target, IdleEngine engine);

        /*
         * Returns all instances of ModifiableProperty which this Effect can be affecting.
         */
        IReadOnlyList<ModifiableProperty> GetAffectableProperties(IdleEngine engine);
    }

    public enum EffectType
    {
        ADD,
        SUBTRACT,
        MULTIPLY
    }
}