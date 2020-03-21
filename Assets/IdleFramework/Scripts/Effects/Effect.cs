using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Effect
    {
        private readonly EffectDefinition definition;
        private readonly IdleEngine engine;
        private bool isActive;
        public Effect(EffectDefinition defintion, IdleEngine engine)
        {
            this.engine = engine;
            this.definition = defintion;
        }

        public EffectDefinition Definition => definition;

        public bool IsActive { get => isActive; set => isActive = value; }

        public object CalculateEffect(ModifiableProperty modifiableProperty)
        {
            return definition.CalculateEffect(modifiableProperty, engine);
        }

        public override string ToString()
        {
            return String.Format("{0} effect '{1}'", IsActive ? "Active" : "Inactive", definition);
        }
    }
}