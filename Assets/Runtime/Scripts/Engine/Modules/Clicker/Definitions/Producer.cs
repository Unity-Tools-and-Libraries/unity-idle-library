using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class Producer : Entity, IBuyable
    {
        public long Id { get; }
        public string Name { get; }
        public string CostExpression { get; }
        public BigDouble UnitOutput { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public BigDouble Quantity { get; set; } = 0;
        public BigDouble OutputMultiplier { get; set; } = 1;
        public BigDouble TotalOutput => UnitOutput * Quantity * OutputMultiplier;
        public Producer(IdleEngine engine, long Id, string name, BigDouble baseCost, BigDouble outputPerSecond, string unlockExpression = null, string enableExpression = null) : base(engine)
        {
            this.Id = Id;
            this.Name = name;
            this.CostExpression = string.Format("return {0}", baseCost);
            this.UnitOutput = outputPerSecond;
            this.UnlockExpression = unlockExpression;
            this.EnableExpression = enableExpression;
        }
        public IDictionary<string, object> Properties { get; set; }

        public static class PropertyNames
        {
            public const string COST = "cost";
            public const string QUANTITY = "quantity";
            public const string OUTPUT_PER_UNIT = "output_per_unit";
            public const string TOTAL_OUTPUT = "total_output";
            public const string UNLOCKED = "unlocked";
            public const string ENABLED = "enabled";
            public const string OUTPUT_MULTIPLIER = "output_multiplier";
            public const string BUYABLE = "buyable";
        }
    }
}