using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class ProducerDefinition : IDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public BigDouble BaseCost { get; }
        public BigDouble OutputPerSecond { get; }

        public ProducerDefinition(string Id, string name, int baseCost, BigDouble outputPerSecond)
        {
            this.Id = Id;
            this.Name = name;
            this.BaseCost = baseCost;
            this.OutputPerSecond = outputPerSecond;
        }
        public IDictionary<string, object> Properties { get; set; }

        public static class PropertyNames
        {
            public const string COST = "cost";
            public const string QUANTITY = "quantity";
            public const string OUTPUT_PER_UNIT = "output_per_unit_per_second";
            public const string TOTAL_OUTPUT = "total_output";
        }
    }
}