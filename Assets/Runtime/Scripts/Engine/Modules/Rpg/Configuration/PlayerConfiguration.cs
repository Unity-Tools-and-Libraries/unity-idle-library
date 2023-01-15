using System;
using System.Collections.Generic;
using BreakInfinity;
using MoonSharp.Interpreter;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class PlayerConfiguration
    {

        /*
         * The type of the Player.
         */
        public Type CharacterType = typeof(RpgCharacter);
        /*
         * The script to initialize the Player after creation.
         * 
         * Use this to add custom 
         */
        public DynValue Initializer;

        /*
         * Script used to determine if an attack hits or misses.
         */
        public DynValue ToHitScript;
        /*
         * 
         */
        public DynValue OnCreatureDiedScript;

        public DynValue ValidationScript;

        public Dictionary<string, BigDouble> Resources;
    }
}