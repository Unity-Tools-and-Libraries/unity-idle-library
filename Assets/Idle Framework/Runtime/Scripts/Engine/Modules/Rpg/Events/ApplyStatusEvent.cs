using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class ApplyStatusEvent
    {
        public const string EventName = "apply_status";
        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The character the status is being applied to."),
            Tuple.Create(typeof(ValueContainer), "The character the status is being applied by."),
            Tuple.Create(typeof(string), "The id of the status."),
            Tuple.Create(typeof(float), "The duration of the newly applied status")
        };
    }
}