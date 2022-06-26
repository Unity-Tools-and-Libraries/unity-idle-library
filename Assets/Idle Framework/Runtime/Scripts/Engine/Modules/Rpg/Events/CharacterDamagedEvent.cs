
using BreakInfinity;
using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections.Generic;

public static class CharacterDamagedEvent
{
    public const string EventName = "character_damaged";
    public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(ValueContainer), "The damaged character."),
            Tuple.Create(typeof(BigDouble), "The amount of damage inflicted.")
        };
}
