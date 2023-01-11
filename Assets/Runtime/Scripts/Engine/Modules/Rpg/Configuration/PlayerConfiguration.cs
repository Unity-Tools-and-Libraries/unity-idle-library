using System;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class PlayerConfiguration
    {
        public PlayerConfiguration()
        {
            ToHitScript = "return ToHit(attacker, defender)";
            Initializer = "InitializePlayer(player)";
            OnCreatureDiedScript = "OnCreatureDied(died)";
            ValidationScript = "ValidatePlayer(player)";
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
        public string ToHitScript;
        /*
         * 
         */
        public string OnCreatureDiedScript;

        public string ValidationScript;
    }
}