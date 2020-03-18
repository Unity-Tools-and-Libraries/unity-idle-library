
using BreakInfinity;

namespace IdleFramework
{
    public class EntityModifierEffect : EffectDefinition
    {
        private readonly IdleEngine engine;
        private string targetEntity;
        private string propertySelector;
        private string subProperty;
        private BigDouble modifierValue;

        public EntityModifierEffect(IdleEngine engine, string modifierEffectKey, string targetEntity, string propertySelector, string subProperty, BigDouble modifierValue)
        {
            this.engine = engine;
            this.targetEntity = targetEntity;
            this.propertySelector = propertySelector;
            this.subProperty = subProperty;
            this.modifierValue = modifierValue;
        }

        
        public BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        
        public bool ShouldApply(GameEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}