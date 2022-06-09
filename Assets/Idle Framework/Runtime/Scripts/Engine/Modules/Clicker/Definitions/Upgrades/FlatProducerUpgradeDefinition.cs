using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades
{
    public class FlatProducerUpgradeDefinition : UpgradeDefinition
    {
        public FlatProducerUpgradeDefinition(string Id, string Name, BigDouble cost, string unlockExpression, string enableExpression, params Tuple<string, string>[] upgradeTargetsAndEffects) : base(Id, Name, cost, unlockExpression, enableExpression, upgradeTargetsAndEffects)
        {
            
        }

        public override IDictionary<string, ContainerModifier> GenerateModifiers()
        {
            return UpgradeTargetsAndEffects
                .ToDictionary(u => u.Item1, u => (ContainerModifier)new AdditiveValueModifier(Id, Name, u.Item2));
        }
    }
}