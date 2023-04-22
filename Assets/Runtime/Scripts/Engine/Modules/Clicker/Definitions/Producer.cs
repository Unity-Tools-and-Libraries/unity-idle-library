using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class Producer : Entity, IBuyable, Definitions.IUnlockable, IEnableable
    {
        public string Name { get; }
        public Dictionary<string, string> CostExpressions { get; }
        public string UnitOutputScript;
        public string UnlockExpression { get; }
        public string EnableExpression { get; }

        // TODO: Implement Builder
        // TODO: Validate cost expresions
        public Producer(IdleEngine engine, long id, string name, Dictionary<string, string> costs, BigDouble unitOutput, string unlockExpression = null, string enableExpression = null) : base(engine, id)
        {
            this.Name = name;
            this.CostExpressions = costs;
            this.UnitOutputScript = "return " + unitOutput;
            this.UnlockExpression = unlockExpression != null ? unlockExpression : "return true";
            this.EnableExpression = enableExpression != null ? enableExpression : "return true";
        }

        public Producer(IdleEngine engine, long id, string name, Dictionary<string, string> costs, string unitOutputScript, string unlockExpression = null, string enableExpression = null) : base(engine, id)
        {
            this.Name = name;
            this.CostExpressions = costs;
            this.UnitOutputScript = unitOutputScript;
            this.UnlockExpression = unlockExpression != null ? unlockExpression : "return true";
            this.EnableExpression = enableExpression != null ? enableExpression : "return true";
        }

        public Producer(IdleEngine engine, long id, string name, Tuple<string, BigDouble> costs, BigDouble unitOutput, string unlockExpression = null, string enableExpression = null) : this(engine, id, name, new Dictionary<string, string>() { { costs.Item1, "return " + costs.Item2.ToString() } }, unitOutput, unlockExpression, enableExpression)
        {
            
        }

        public Producer(IdleEngine engine, long id, string name, Tuple<string, BigDouble> costs, string unitOutput, string unlockExpression = null, string enableExpression = null) : this(engine, id, name, new Dictionary<string, string>() { { costs.Item1, "return " + costs.Item2.ToString() } }, unitOutput, unlockExpression, enableExpression)
        {

        }

        public Producer(IdleEngine engine, long id, string name, Tuple<string, string> costs, BigDouble unitOutput, string unlockExpression = null, string enableExpression = null) : this(engine, id, name, new Dictionary<string, string>() { { costs.Item1, costs.Item2 } }, unitOutput, unlockExpression, enableExpression)
        {

        }

        public Producer(IdleEngine engine, long id, string name, Tuple<string, string> costs, string unitOutput, string unlockExpression = null, string enableExpression = null) : this(engine, id, name, new Dictionary<string, string>() { { costs.Item1, costs.Item2 } }, unitOutput, unlockExpression, enableExpression)
        {

        }

        public static class PropertyNames
        {
            public const string COST = "cost";
            public const string OUTPUT_PER_UNIT = "output_per_unit";
            public const string TOTAL_OUTPUT = "total_output";
            public const string UNLOCKED = "unlocked";
            public const string ENABLED = "enabled";
            public const string OUTPUT_MULTIPLIER = "output_multiplier";
            public const string BUYABLE = "buyable";
        }

        public void Unlock()
        {
            throw new NotImplementedException();
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }
    }
}