using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat
{
    public static class DamageTakenEvent
    {
        public const string EventName = "damage_taken";

        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(Character), "The damaged character."),
            Tuple.Create(typeof(Character), "The character dealing the damage."),
            Tuple.Create(typeof(BigDouble), "The amount of damage.")
        };
    }
}