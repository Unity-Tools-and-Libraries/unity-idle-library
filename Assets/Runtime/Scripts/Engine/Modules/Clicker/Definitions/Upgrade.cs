using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using System;
using System.Linq;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions
{
    public class Upgrade : EntityModifier<ClickerPlayer>, IEnableable, IUnlockable, IBuyable
    {
        private bool isUnlocked;
        private bool isEnabled;
        public string Name { get; }
        public Dictionary<string, string> CostExpressions { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
        public BigDouble Quantity { get; set; } = 0;
        public BigDouble MaxQuantity { get; }

        public bool IsUnlocked
        {
            get
            {
                return isUnlocked;
            }
            set
            {
                if (value != isUnlocked)
                {
                    isUnlocked = value;
                    var changeEvent = new IsUnlockedChangeEvent(this);
                    Emit(IsUnlockedChangeEvent.EventName, changeEvent);
                    Engine.Emit(IsUnlockedChangeEvent.EventName, changeEvent);
                }
                isUnlocked = value;
            }
        }
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    var changeEvent = new IsEnabledChangedEvent(this);
                    Emit(IsEnabledChangedEvent.EventName, changeEvent);
                    Engine.Emit(IsEnabledChangedEvent.EventName, changeEvent);
                }
            }
        }

        public Upgrade(IdleEngine engine, long Id, string Name, Tuple<string, BigDouble> cost, string unlockExpression, string enableExpression, Dictionary<string, Tuple<string, string>> effects, BigDouble? maximumLevels = null, Dictionary<string, object> extraProperties = null) : this(engine, Id, Name, new Dictionary<string, string>() { { cost.Item1, "return " + cost.Item2.ToString() } }, unlockExpression, enableExpression, effects, maximumLevels, extraProperties)
        {
            
        }

        public Upgrade(IdleEngine engine, long Id, string Name, Tuple<string, string> cost, string unlockExpression, string enableExpression, Dictionary<string, Tuple<string, string>> effects, BigDouble? maximumLevels = null, Dictionary<string, object> extraProperties = null) : this(engine, Id, Name, new Dictionary<string, string>() { { cost.Item1, cost.Item2 } }, unlockExpression, enableExpression, effects, maximumLevels, extraProperties)
        {

        }

        // TODO: Implement validation of cost expressions.
        public Upgrade(IdleEngine engine, long Id, string Name, Dictionary<string, string> costs, string unlockExpression, string enableExpression, Dictionary<string, Tuple<string, string>> effects, BigDouble? maximumLevels = null, Dictionary<string, object> extraProperties = null) : base(engine, Id, effects, extraProperties)
        {
            this.Name = Name;
            this.CostExpressions = costs;
            this.UnlockExpression = unlockExpression;
            this.EnableExpression = enableExpression;
            this.MaxQuantity = maximumLevels.HasValue ? maximumLevels.Value : BigDouble.One;
        }
    }
}