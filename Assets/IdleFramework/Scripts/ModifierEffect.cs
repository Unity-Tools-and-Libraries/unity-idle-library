using BreakInfinity;

namespace IdleFramework
{
    public struct ModifierEffect
    {
        public readonly Modifier modifier;
        public readonly Effect effect;

        public ModifierEffect(Modifier modifier, Effect effect)
        {
            this.modifier = modifier;
            this.effect = effect;
        }

        public object Apply(ModifiableProperty modifiableProperty, IdleEngine engine)
        {
            if (modifier.IsActive(engine))
            {
                return effect.CalculateEffect(modifiableProperty);
            } else
            {
                return modifiableProperty.RawValue(engine);
            }
        }
    }
}