using System;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class CharacterConfiguration
    {
        public CharacterConfiguration()
        {
            PlayerInitializer = Resources.Load<TextAsset>("Lua/Rpg/DefaultPlayerInitializer").text;
            AttackToHitScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultToHitScript").text;
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
        public string PlayerInitializer;

        /*
         * Script used to determine which a characters attacks hit or miss.
         */
        public string AttackToHitScript;
    }
}