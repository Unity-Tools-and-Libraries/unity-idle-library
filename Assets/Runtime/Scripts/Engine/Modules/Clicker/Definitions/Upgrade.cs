using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events;
using System;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions
{
    public class Upgrade : EntityModifier<ClickerPlayer>, IEnableable, IUnlockable, IBuyable
    {
        private bool isUnlocked;
        private bool isEnabled;
        public long Id { get; }
        public string Name { get; }
        public string CostExpression { get; }
        public string UnlockExpression { get; }
        public string EnableExpression { get; }
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