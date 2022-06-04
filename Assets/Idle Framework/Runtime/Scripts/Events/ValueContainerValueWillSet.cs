using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public class ValueContainerValueWillSet : IdleEngineEvent
    {
        public readonly object CurrentValue;
        public readonly object NextValue;
        public readonly ValueContainer Container;
        public ValueContainerValueWillSet(ValueContainer container, object currentValue, object nextValue)
        {
            this.CurrentValue = currentValue;
            this.Container = container;
            this.PreventBubbling = true;
            this.NextValue = nextValue;
        }


        public bool PreventBubbling { get; set; }
    }
}
