
using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class EntityModifierEffectDefinition : EffectDefinition
    {
        private string targetEntity;
        private string propertySelector;
        private string subProperty;
        private BigDouble modifierValue;

        public EntityModifierEffectDefinition(string modifierEffectKey, string targetEntity, string propertySelector, string subProperty, BigDouble modifierValue)
        {
            this.targetEntity = targetEntity;
            this.propertySelector = propertySelector;
            this.subProperty = subProperty;
            this.modifierValue = modifierValue;
        }

        
        public object CalculateEffect(ModifiableProperty target, IdleEngine engine)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<ModifiableProperty> GetAffectableProperties(IdleEngine engine)
        {
            List<ModifiableProperty> affected = new List<ModifiableProperty>();
            GameEntity entity;
            if(engine.AllEntities.TryGetValue(targetEntity, out entity))
            {
                switch (propertySelector)
                {
                    case "outputs":
                        affected.Add(entity.Outputs[subProperty]);
                        break;
                    case "inputs":
                        affected.Add(entity.Inputs[subProperty]);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            return affected.AsReadOnly();
        }
    }
}