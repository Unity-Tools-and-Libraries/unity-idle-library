using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class Modifier : Updates
    {
        private readonly ModifierDefinition definition;
        private bool isActive;
        public bool IsActive => isActive;

        public Modifier(ModifierDefinition definition)
        {
            this.definition = definition;
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            isActive = definition.IsActiveMatcher.Matches(engine);
        }
    }
}