using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class CharacterDiedEvent : ScriptingContext
    {
        public const string EventName = "character_died";
        public static readonly List<Tuple<Type, String>> Arguments = new List<Tuple<Type, String>>()
        {
            Tuple.Create(typeof(Character), "The dead character")
        };

        public Dictionary<string, object> GetScriptingContext(string contextType = null)
        {
            throw new NotImplementedException();
        }
    }
}