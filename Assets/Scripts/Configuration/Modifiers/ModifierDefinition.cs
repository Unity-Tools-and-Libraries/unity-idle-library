using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{    
    public interface ModifierDefinition<T> : Updates
    {
        T CalculateEffect(IdleEngine engine);
    }

    public enum EffectType
    {
        ADD,
        SUBTRACT,
        MULTIPLY
    }
}