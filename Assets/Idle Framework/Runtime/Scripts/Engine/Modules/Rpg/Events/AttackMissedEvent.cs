using io.github.thisisnozaku.idle.framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackMissedEvent
{
    public const string EventName = "attack_missed";
    public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The attacking character."),
            Tuple.Create(typeof(string), "The attack failure reason message."),
        };
}
