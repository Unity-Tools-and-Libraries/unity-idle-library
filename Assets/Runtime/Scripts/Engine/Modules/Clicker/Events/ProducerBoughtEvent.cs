using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Events
{
    public class ProducerBoughtEvent : EntityBoughtEvent<Producer>
    {
        public const string EventName = "ProducerBought";
        public ProducerBoughtEvent(Producer bought) : base(bought)
        {
        }
    }
}