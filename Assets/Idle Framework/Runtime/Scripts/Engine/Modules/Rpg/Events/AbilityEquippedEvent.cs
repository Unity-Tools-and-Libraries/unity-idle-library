using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEquippedEvent
{
    public const string EventName = "ability_equipped";
    public static readonly List<Tuple<Type, string>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(string), "The id of the equipped ability."),
        };
}
