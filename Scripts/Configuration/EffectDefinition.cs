using BreakInfinity;

namespace IdleFramework
{    
    public interface EffectDefinition<T>
    {
        void ApplyEffect(IdleEngine engine, T source);
    }

    public enum EffectType
    {
        ADD,
        SUBTRACT,
        MULTIPLY
    }
}