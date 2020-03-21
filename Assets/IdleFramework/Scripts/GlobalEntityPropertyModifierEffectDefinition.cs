using BreakInfinity;
using IdleFramework;

namespace IdleFramework
{
    public class GlobalEntityPropertyModifierEffectDefinition : EntityPropertyModifierEffectDefinition
    {
        bool addIfMissing = false;
        public GlobalEntityPropertyModifierEffectDefinition(string entityProperty, string entitySubProperty, ValueContainer value, EffectType type, bool addIfMissing) : base("*", entityProperty, entitySubProperty, value, type)
        {
            this.addIfMissing = addIfMissing;
        }

        public GlobalEntityPropertyModifierEffectDefinition(string entityProperty, string entitySubProperty, ValueContainer value, EffectType type) : this(entityProperty, entitySubProperty, value, type, false) { }
    }
}