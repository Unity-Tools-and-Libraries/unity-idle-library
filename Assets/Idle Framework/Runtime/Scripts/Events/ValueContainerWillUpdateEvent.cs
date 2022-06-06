using io.github.thisisnozaku.idle.framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public class ValueContainerWillUpdateEvent
    {
        public readonly ValueContainer UpdatingContainer;
        public readonly object PreviousValue;
        public readonly float Time;

        public ValueContainerWillUpdateEvent(ValueContainer updatingContainer, float time, object previousValue)
        {
            UpdatingContainer = updatingContainer;
            PreviousValue = previousValue;
            this.Time = time;
            PreventBubbling = false;
        }

        public bool PreventBubbling { get; set; }
    }
}