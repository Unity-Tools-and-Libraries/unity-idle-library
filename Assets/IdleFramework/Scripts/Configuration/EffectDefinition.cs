using BreakInfinity;

namespace IdleFramework
{    
    public interface EffectDefinition
    {
        BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine);
    }

    public enum EffectType
    {
        ADD,
        SUBTRACT,
        MULTIPLY
    }
}