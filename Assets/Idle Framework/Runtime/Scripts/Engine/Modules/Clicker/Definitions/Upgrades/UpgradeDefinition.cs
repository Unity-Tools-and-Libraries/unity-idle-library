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
        public string Name { get; }
        public IDictionary<string, object> Properties { get; }
        public Tuple<string, string>[] UpgradeTargetsAndEffects { get; }
        public BigDouble Cost { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public UpgradeDefinition(string Id, string Name, BigDouble cost, string unlockExpression, string enableExpression, params Tuple<string, string>[] targetsAndEffects)
        {
            this.Id = Id;
            this.Cost = cost;
            UpgradeTargetsAndEffects = targetsAndEffects;
            this.UnlockExpression = unlockExpression;
            this.EnableExpression = enableExpression;
        }

        public abstract IDictionary<string, ContainerModifier> GenerateModifiers();
    }
}