using BreakInfinity;

using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions
{
    public abstract class Upgrade : IEnableable, IUnlockable, IBuyable
    {
        public string Id { get; }
        public string Name { get; }
        public IDictionary<string, object> Properties { get; }
        public Tuple<string, string>[] UpgradeTargetsAndEffects { get; }
        public string CostExpression { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public Upgrade(string Id, string Name, BigDouble cost, string unlockExpression, string enableExpression, params Tuple<string, string>[] targetsAndEffects)
        {
            this.Id = Id;
            this.Name = Name;
            this.CostExpression = cost.ToString();
            UpgradeTargetsAndEffects = targetsAndEffects;
            this.UnlockExpression = unlockExpression;
            this.EnableExpression = enableExpression;
        }
    }
}