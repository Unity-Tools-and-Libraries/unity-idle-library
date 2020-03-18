using System;
using System.Collections.Generic;

namespace IdleFramework
{
    internal class ModifierEffectCache
    {
        private readonly Dictionary<Type, Dictionary<ModifierEffectCacheSelector, ModifierEffect>> cache;
        public ModifierEffectCache()
        {
        }

        private struct ModifierEffectCacheSelector
        {
            public string subject;
            public ModifierDefinition modifier;
            public EffectDefinition effect;

            public ModifierEffectCacheSelector(string subject, ModifierDefinition modifier, EffectDefinition effect)
            {
                this.subject = subject;
                this.modifier = modifier;
                this.effect = effect;
            }
        }
    }
}