using System.Linq;
using System.Collections.Generic;
using BreakInfinity;
using System;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades
{
    public class FlatProducerUpgradeDefinition : Upgrade
    {
        public FlatProducerUpgradeDefinition(string Id, string Name, BigDouble cost, string unlockExpression, string enableExpression, params Tuple<string, string>[] upgradeTargetsAndEffects) : base(Id, Name, cost, unlockExpression, enableExpression, upgradeTargetsAndEffects)
        {
            
        }
    }
}