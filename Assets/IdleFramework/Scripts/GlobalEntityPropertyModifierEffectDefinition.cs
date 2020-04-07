using BreakInfinity;
using IdleFramework;

namespace IdleFramework
{
    public class GlobalEntityPropertyModifierEffectDefinition : EntityPropertyModifierEffectDefinition
    {
        bool addIfMissing = false;
        public GlobalEntityPropertyModifierEffectDefinition(string entityProperty, ValueContainer value, EffectType type, bool addIfMissing) : base("*", entityProperty, value, type)
        {
            this.addIfMissing = addIfMissing;
        }

        public GlobalEntityPropertyModifierEffectDefinition(string entityProperty, ValueContainer value, EffectType type) : this(entityProperty, value, type, false) { }
    }
}