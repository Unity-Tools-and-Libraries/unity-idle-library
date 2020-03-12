using BreakInfinity;

namespace IdleFramework
{
    public abstract class EntityEffect : EffectDefinition<GameEntity>
    {
        public abstract void ApplyEffect(IdleEngine engine);
    }
}