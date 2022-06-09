using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System;
using System.Collections.Generic;
using System.Linq;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades
{
    public class MultiplierProducerUpgradeDefinition : UpgradeDefinition
    {
        public MultiplierProducerUpgradeDefinition(string Id, string Name, BigDouble cost, string enableExpression, string unlockExpression, params Tuple<string, string>[] upgradeTargetsAndEffects) 
            : base(Id, Name, cost, enableExpression, unlockExpression, upgradeTargetsAndEffects)
        {
            
        }

        public override IDictionary<string, ContainerModifier> GenerateModifiers()
        {
            return UpgradeTargetsAndEffects
                .ToDictionary(u => u.Item1, u => (ContainerModifier)new MultiplicativeValueModifier(Id, Name, u.Item2, new string[] { u.Item1 }));
        }
    }
}