using BreakInfinity;
using System;
using System.Collections.Generic;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public static class DamageInflictedEvent
    {
        public const string EventName = "damage_inflicted";

        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(Character), "The damaging character."),
            Tuple.Create(typeof(BigDouble), "The amount of damage.")
        };
    }
}