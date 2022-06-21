using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CharacterActedEvent
    {
        public const string EventName = "character_acted";
        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(Character), "The character who is acting.")
        };
    }
}