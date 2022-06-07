using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions
{
    public abstract class UpgradeDefinition : IDefinition
    {
        public string Id { get; }

        public IDictionary<string, object> Properties { get; }
        public string[] UpgradeTargets { get; }
        public BigDouble Cost { get; }
        public UpgradeDefinition(string Id, string Name, BigDouble cost, string[] upgradeTargets)
        {
            this.Id = Id;
            this.Cost = cost;
            UpgradeTargets = upgradeTargets;
        }

        public abstract ContainerModifier GenerateModifier();
    }
}