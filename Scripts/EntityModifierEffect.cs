
using BreakInfinity;

namespace IdleFramework
{
    public class EntityModifierEffect : EffectDefinition<GameEntity>
    {
        private string targetEntity;
        private GameEntity.ModifiableProperty propertySelector;
        private string subProperty;
        private BigDouble modifierValue;

        public EntityModifierEffect(string modifierEffectKey, string targetEntity, GameEntity.ModifiableProperty propertySelector, string subProperty, BigDouble modifierValue)
        {
            this.targetEntity = targetEntity;
            this.propertySelector = propertySelector;
            this.subProperty = subProperty;
            this.modifierValue = modifierValue;
        }

        public void ApplyEffect(IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldApply(IdleEngine engine, GameEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}