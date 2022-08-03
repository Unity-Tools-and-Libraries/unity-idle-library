using System;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class PlayerConfiguration
    {
        public PlayerConfiguration()
        {
            AttackScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultToHitScript").text;
            Initializer = Resources.Load<TextAsset>("Lua/Rpg/DefaultPlayerInitializer").text;
            OnCreatureDiedScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultCreatureDiedScript").text;
            ValidationScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultPlayerValidationScript").text;
        }

        /*
         * The type of the Player.
         */
        public Type CharacterType = typeof(RpgCharacter);
        /*
         * The script to initialize the Player after creation.
         * 
         * Use this to add custom 
         */
        public string Initializer;

        /*
         * Script used to determine if an attack hits or misses.
         */
        public string AttackScript;
        /*
         * 
         */
        public string OnCreatureDiedScript;

        public string ValidationScript;
    }
}