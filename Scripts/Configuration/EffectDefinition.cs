using BreakInfinity;

namespace IdleFramework
{
    public interface EffectDefinition<T>
    {
        void ApplyEffect(IdleEngine engine);
    }

    public enum EffectType
    {
        ADD,
        SUBTRACT,
        MULTIPLY
    }
}