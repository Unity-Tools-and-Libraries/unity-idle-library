using io.github.thisisnozaku.idle.framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Events
{
    public static class ValueContainerWillUpdateEvent
    {
        public const string EventName = "value_changed";
        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The container that will update."),
            Tuple.Create(typeof(float), "The amount of time passed since the last update."),
            Tuple.Create(typeof(object), "The current value of the container, before update.")
        };
    }
}