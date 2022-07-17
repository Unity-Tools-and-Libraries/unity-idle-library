using BreakInfinity;

using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker
{
    public class Producer : Entity, IBuyable, Definitions.IUnlockable, IEnableable
    {
        public string Name { get; }
        public string CostExpression { get; }
        public string UnitOutputScript;
        public BigDouble UnitOutput => Engine.Scripting.Evaluate(UnitOutputScript).ToObject<BigDouble>();
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public BigDouble Quantity { get; set; } = 0;
        public BigDouble OutputMultiplier { get; set; } = 1;
        public BigDouble TotalOutput => UnitOutput * OutputMultiplier * Quantity;
        public Producer(IdleEngine engine, long id, string name, BigDouble baseCost, BigDouble unitOutput, string unlockExpression = null, string enableExpression = null) : base(engine, id)
        {
            this.Name = name;
            this.CostExpression = string.Format("return {0}", baseCost);
            this.UnitOutputScript = "return " + unitOutput;
            this.UnlockExpression = unlockExpression != null ? unlockExpression : "return true";
            this.EnableExpression = enableExpression != null ? enableExpression : "return true";
        }
        public Producer(IdleEngine engine, long id, string name, BigDouble baseCost, string unitOutputScript, string unlockExpression = null, string enableExpression = null) : base(engine, id)
        {
            this.Name = name;
            this.CostExpression = string.Format("return {0}", baseCost);
            this.UnitOutputScript = unitOutputScript;
            this.UnlockExpression = unlockExpression != null ? unlockExpression : "return true";
            this.EnableExpression = enableExpression != null ? enableExpression : "return true";
        }
        public IDictionary<string, object> Properties { get; set; }
        private bool isUnlocked;
        private bool isEnabled;
        public bool IsUnlocked { get
            {
                return isUnlocked;
            } set { 
                if(value != isUnlocked)
                {
                    var changeEvent = new IsUnlockedChangeEvent(this);
                    Emit(IsUnlockedChangeEvent.EventName, changeEvent);
                    Engine.Emit(IsUnlockedChangeEvent.EventName, changeEvent);
                }
                isUnlocked = value;
            } 
        }
        public bool IsEnabled { get
            {
                return isEnabled;
            } set {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    var changeEvent = new IsEnabledChangedEvent(this);
                    Emit(IsEnabledChangedEvent.EventName, changeEvent);
                    Engine.Emit(IsEnabledChangedEvent.EventName, changeEvent);
                }
            } 
        }

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