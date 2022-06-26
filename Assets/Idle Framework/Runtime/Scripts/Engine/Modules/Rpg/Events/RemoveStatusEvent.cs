using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveStatusEvent
{
    public const string EventName = "status_removed";
    public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The character the status was removed from."),
            Tuple.Create(typeof(string), "The id of the removed status."),
        };
    
}
