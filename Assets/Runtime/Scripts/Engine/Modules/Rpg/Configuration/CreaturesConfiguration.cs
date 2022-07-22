using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class CreaturesConfiguration
    {
        public CreaturesConfiguration()
        {
            AttackScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultToHitScript").text;
            Initializer = Resources.Load<TextAsset>("Lua/Rpg/DefaultCreatureInitializer").text;
            XpValueCalculationScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultXpValueScript").text;
            GoldValueCalculationScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultGoldValueScript").text;
            AttributeScalingScript = Resources.Load<TextAsset>("Lua/Rpg/DefaultAttributeScalingScript").text;
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
         * Script used to determine how much xp a creature is worth when defeated.
         */
        public string XpValueCalculationScript;
        /*
         * Script used to determine how much xp a creature is worth when defeated.
         */
        public string GoldValueCalculationScript;
        /*
         * 
         */
        public string AttributeScalingScript;
    }
}