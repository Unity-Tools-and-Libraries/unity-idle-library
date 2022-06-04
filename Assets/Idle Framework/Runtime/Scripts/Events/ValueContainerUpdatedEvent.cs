using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public class ValueContainerUpdatedEvent : IdleEngineEvent
    {
        public readonly ValueContainer Container;

        public ValueContainerUpdatedEvent(ValueContainer container)
        {
            Container = container;
            PreventBubbling = false;
        }

        public bool PreventBubbling { get; set; }
    }
}