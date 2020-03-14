using BreakInfinity;

namespace IdleFramework
{
    public abstract class EntityEffect : EffectDefinition<ModifierDefinition>
    {
        public abstract void ApplyEffect(IdleEngine engine, ModifierDefinition source);
    }
}