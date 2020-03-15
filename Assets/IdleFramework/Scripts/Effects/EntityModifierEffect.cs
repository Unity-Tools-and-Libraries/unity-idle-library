
using BreakInfinity;

namespace IdleFramework
{
    public class EntityModifierEffect : EffectDefinition<ModifierDefinition>
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

        public void ApplyEffect(IdleEngine engine, ModifierDefinition parentModifier)
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldApply(IdleEngine engine, GameEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}