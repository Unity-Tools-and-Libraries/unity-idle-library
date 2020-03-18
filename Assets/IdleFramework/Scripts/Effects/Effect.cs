using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Effect
    {
        private readonly EffectDefinition defintion;
        private readonly IdleEngine engine;
        private bool isActive;
        public Effect(EffectDefinition defintion, IdleEngine engine)
        {
            this.engine = engine;
            this.defintion = defintion;
        }

        public EffectDefinition Defintion => defintion;

        public bool IsActive { get => isActive; set => isActive = value; }

        public BigDouble CalculateEffect(ModifiableProperty modifiableProperty)
        {
            return this.defintion.CalculateEffect(modifiableProperty, engine);
        }
    }
}