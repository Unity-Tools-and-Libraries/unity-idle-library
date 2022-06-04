using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events {
    public class ChildValueChangedEvent : ValueChangedEvent
    {
        public ChildValueChangedEvent(string path, object previousValue, object newValue, ValueContainer source) : base(path, previousValue, newValue, source)
        {
        }
    }
}