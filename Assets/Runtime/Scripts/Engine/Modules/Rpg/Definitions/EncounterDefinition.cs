
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class EncounterDefinition
    {
        public long Id { get; }
        /**
         * Which creatures are in the encounter.
         * 
         * The first parameter of each tuple is the id of the creature. 
         * The second parameter is the level modifier to the level of the encounter. (e.g., 0 means level of the encounter, 1 is 1 higher, -1 is 1 lower, and so on)
         */
        public readonly Tuple<long, int>[] CreatureOptions;
        
        public EncounterDefinition(long id, params Tuple<long, int>[] options)
        {
            this.Id = id;
            if(options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.CreatureOptions = options;
        }
    }
}