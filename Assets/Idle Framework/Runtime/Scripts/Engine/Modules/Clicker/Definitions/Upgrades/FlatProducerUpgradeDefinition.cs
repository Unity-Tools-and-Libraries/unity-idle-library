using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades
{
    public class FlatProducerUpgradeDefinition : UpgradeDefinition
    {
        public BigDouble Value { get; }
        public FlatProducerUpgradeDefinition(string Id, string Name, BigDouble cost, BigDouble value, params ProducerDefinition[] upgradeTargets) : base(Id, Name, cost, upgradeTargets.Select(x => x.Id).ToArray())
        {
            this.Value = value;
        }

        public override ContainerModifier GenerateModifier()
        {
            return new AdditiveValueModifier(Id, Id, Value);
        }
    }
}