using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration;
using MoonSharp.Interpreter;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public partial class RpgModule
    {
        private CharacterAdvancementConfiguration Advancement;
        /**
         * Configure the module for character improvement via buying individual 
         * attributes.
         */
        public CharacterAdvancementConfiguration ConfigureAttributeAdvancement()
        {
            Player.Advancement = new AttributeEnhancementConfiguration(this);
            return Player.Advancement;
        }

        public class AttributeEnhancementConfiguration : CharacterAdvancementConfiguration
        {
            private RpgModule rpgModule;
            public AttributeEnhancementConfiguration(RpgModule rpgModule)
            {
                this.rpgModule = rpgModule;
            }
            public AttributeEnhancementConfiguration UpgradeableAttribute(string attributeName, Dictionary<string, DynValue> resourceCostCalculators)
            {
                rpgModule.Advancement.DefineUpgradeableAttribute(attributeName, resourceCostCalculators);
                return this;
            }
        }
    }
}
