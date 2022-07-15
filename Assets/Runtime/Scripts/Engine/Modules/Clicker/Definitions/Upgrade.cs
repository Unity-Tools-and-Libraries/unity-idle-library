using BreakInfinity;

using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions
{
    public class Upgrade : EntityModifier<ClickerPlayer>, IEnableable, IUnlockable, IBuyable
    {
        public long Id { get; }
        public string Name { get; }
        public string CostExpression { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public Upgrade(IdleEngine engine, long Id, string Name, BigDouble cost, string unlockExpression, string enableExpression, Dictionary<string, Tuple<string, string>> effects) : base(engine, Id, effects)
        {
            this.Id = Id;
            this.Name = Name;
            this.CostExpression = string.Format("return {0}", cost);
            this.UnlockExpression = unlockExpression;
            this.EnableExpression = enableExpression;
        }
    }
}