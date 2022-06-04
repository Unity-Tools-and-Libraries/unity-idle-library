using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    /*
     * Event fired when the value in a container changes.
     */
    public class ValueChangedEvent : IdleEngineEvent
    {
        public readonly string Path;
        public readonly object NewValue;
        public readonly object OldValue;
        public readonly ValueContainer Source;

        public ValueChangedEvent(string path, object previousValue, object newValue, ValueContainer source)
        {
            Path = path;
            NewValue = newValue;
            OldValue = previousValue;
            Source = source;
            PreventBubbling = true;
        }

        public bool PreventBubbling { get; set; }
    }
}