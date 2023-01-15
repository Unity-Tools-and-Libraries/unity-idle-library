using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using MoonSharp.Interpreter;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class CharacterAdvancementConfiguration
    {
        public void DoConfiguration(IdleEngine engine) { }
        private Dictionary<string, Dictionary<string, DynValue>> AttributePriceCalculators = new Dictionary<string, Dictionary<string, DynValue>>();

        public void DefineUpgradeableAttribute(string attributeName, Dictionary<string, DynValue> resourceCostCalculators)
        {
            AttributePriceCalculators[attributeName] = resourceCostCalculators;
        }

        public Dictionary<string, DynValue> GetAttributePurchaseCalculator(string attribute)
        {
            Dictionary<string, DynValue> calculators;
            if(!AttributePriceCalculators.TryGetValue(attribute, out calculators))
            {
                throw new InvalidOperationException(string.Format("No calculators for the attribute {0} were found", attribute));
            }
            return calculators;
        }
    }
}