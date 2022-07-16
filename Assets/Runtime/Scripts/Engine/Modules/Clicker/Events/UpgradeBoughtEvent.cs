using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events
{
    public class UpgradeBoughtEvent : EntityBoughtEvent<Upgrade>
    {
        public const string EventName = "UpgradeBought";
        public UpgradeBoughtEvent(Upgrade bought) : base(bought)
        {
        }
    }
}