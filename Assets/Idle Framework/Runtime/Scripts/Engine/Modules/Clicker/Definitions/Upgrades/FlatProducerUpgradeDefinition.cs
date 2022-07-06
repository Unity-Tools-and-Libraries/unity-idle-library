using System.Linq;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades
{
    public class FlatProducerUpgradeDefinition : UpgradeDefinition
    {
        public FlatProducerUpgradeDefinition(string Id, string Name, BigDouble cost, string unlockExpression, string enableExpression, params Tuple<string, string>[] upgradeTargetsAndEffects) : base(Id, Name, cost, unlockExpression, enableExpression, upgradeTargetsAndEffects)
        {
            
        }

        public override IDictionary<string, ContainerModifier> GenerateModifiers(IdleEngine engine)
        {
            return UpgradeTargetsAndEffects
                .ToDictionary(u => u.Item1, u =>
                {
                    var target = engine.GetProperty(u.Item1);
                    return (ContainerModifier)new ValueModifier(Id + u.Item1, Name, "return " + u.Item2, target, priority: ValueModifier.DefaultPriorities.MULTIPLICATION + 1);
                });
        }
    }
}