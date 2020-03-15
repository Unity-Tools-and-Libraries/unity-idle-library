using BreakInfinity;
using IdleFramework;

namespace IdleFramework
{
    public class GlobalEntityPropertyModifierEffect : EntityPropertyModifierEffect
    {
        bool addIfMissing = false;
        public GlobalEntityPropertyModifierEffect(string entityProperty, string entitySubProperty, BigDouble value, EffectType type, bool addIfMissing) : base(null, entityProperty, entitySubProperty, value, type)
        {
            this.addIfMissing = addIfMissing;
        }

        public GlobalEntityPropertyModifierEffect(string entityProperty, string entitySubProperty, BigDouble value, EffectType type) : this(entityProperty, entitySubProperty, value, type, false) { }

        public override void ApplyEffect(IdleEngine engine, ModifierDefinition parentModifier)
        {
            foreach(var entity in engine.AllEntities.Values)
            {
                if (addIfMissing || entityHasPropertyAndSubproperty(entity, EntityProperty, EntitySubProperty))
                {
                    ApplyEffectToEntity(entity, engine, parentModifier);
                }
            }
        }
    }
}