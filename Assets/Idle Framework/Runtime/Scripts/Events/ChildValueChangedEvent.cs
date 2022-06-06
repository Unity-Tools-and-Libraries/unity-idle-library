using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events {
    public static class ChildValueChangedEvent
    {
        public static readonly string EventName = "child_value_changed";
        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The container whose value changed."),
            Tuple.Create(typeof(string), "The path to the changed value."),
            Tuple.Create(typeof(object), "The previous raw value."),
            Tuple.Create(typeof(object), "The new raw value")
        };
    }
}