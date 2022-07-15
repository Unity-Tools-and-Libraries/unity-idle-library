
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class EncounterDefinition
    {
        private long id;
        public long Id => id;
        public readonly Tuple<long, int>[] CreatureOptions;
        
        public EncounterDefinition(long id, params Tuple<long, int>[] options)
        {
            this.id = id;
            this.CreatureOptions = options;
        }
    }
}