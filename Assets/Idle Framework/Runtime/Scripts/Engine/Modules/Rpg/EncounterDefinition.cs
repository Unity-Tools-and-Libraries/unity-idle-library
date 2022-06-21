using io.github.thisisnozaku.idle.framework.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class EncounterDefinition : IDefinition
    {
        private string id;
        public string Id => id;
        public readonly Tuple<string, int>[] CreatureOptions;

        public IDictionary<string, object> Properties => throw new System.NotImplementedException();
        /*
         * 
         */
        public EncounterDefinition(string id, params Tuple<string, int>[] options)
        {
            this.id = id;
            this.CreatureOptions = options;
        }
    }
}