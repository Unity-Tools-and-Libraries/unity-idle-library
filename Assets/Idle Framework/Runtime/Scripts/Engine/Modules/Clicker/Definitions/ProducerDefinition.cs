using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ProducerDefinition : IDefinition, IUnlockable, IEnableable, IBuyable
    {
        public string Id { get; }
        public string Name { get; }
        public string CostExpression { get; }
        public BigDouble OutputPerSecond { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public ProducerDefinition(string Id, string name, BigDouble baseCost, BigDouble outputPerSecond, string unlockExpression = null, string enableExpression = null)
        {
            this.Id = Id;
            this.Name = name;
            this.CostExpression = baseCost.ToString();
            this.OutputPerSecond = outputPerSecond;
            this.UnlockExpression = unlockExpression;
            this.EnableExpression = enableExpression;
        }
        public IDictionary<string, object> Properties { get; set; }

        public static class PropertyNames
        {
            public const string COST = "cost";
            public const string QUANTITY = "quantity";
            public const string OUTPUT_PER_UNIT = "output_per_unit_per_second";
            public const string TOTAL_OUTPUT = "total_output";
            public const string UNLOCKED = "unlocked";
            public const string ENABLED = "enabled";
            public const string OUTPUT_MULTIPLIER = "output_multiplier";
            public const string BUYABLE = "buyable";
        }
    }
}