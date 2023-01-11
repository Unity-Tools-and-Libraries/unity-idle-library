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
            ToHitScript = "return ToHit(attacker, defender)";
            Initializer = "InitializeCreature(creature, definition, level)";
            XpValueCalculationScript = "return CalculateXpValue(creature, level)";
            GoldValueCalculationScript = "return CalculateGoldValue(creature, level)";
            AttributeScalingScript = "return ScaleAttribute(level)";
            ValidatorScript = "if(creature.maximumHealth.Total <= 0) then error('creature health must be at least 1') end";
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
        /**
         * Script used to validate creatures on creation.
         */
        public string ValidatorScript;
    }
}