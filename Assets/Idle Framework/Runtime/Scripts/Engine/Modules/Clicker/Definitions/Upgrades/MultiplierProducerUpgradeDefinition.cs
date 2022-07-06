using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
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

        public override IDictionary<string, ContainerModifier> GenerateModifiers(IdleEngine engine)
        {
            return UpgradeTargetsAndEffects
                .ToDictionary(u => u.Item1, u =>
                {
                    var target = engine.GetProperty(u.Item1);
                    return (ContainerModifier)new ValueModifier(Id + u.Item1, Name, "return " + u.Item2, target, priority: ValueModifier.DefaultPriorities.ADDITION + 1);
                });
        }
    }
}