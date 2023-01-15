using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Configuration
{
    public class CreaturesConfiguration
    {
        public CreaturesConfiguration()
        {
            ValidatorScript = DynValue.NewString("if(creature.maximumHealth.Total <= 0) then error('creature health must be at least 1') end");
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
        public DynValue Initializer;

        /*
         * Script used to determine if an attack hits or misses.
         */
        public DynValue ToHitScript;
        /*
         * Script used to determine how much xp a creature is worth when defeated.
         */
        public DynValue XpValueCalculationScript;
        /*
         * Script used to determine how much xp a creature is worth when defeated.
         */
        public DynValue GoldValueCalculationScript;
        /*
         * 
         */
        public DynValue AttributeScalingScript;
        /**
         * Script used to validate creatures on creation.
         */
        public DynValue ValidatorScript;
    }
}