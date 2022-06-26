using io.github.thisisnozaku.idle.framework.Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public class ItemDefinition : IDefinition
    {
        public string Id { get; }
        public IDictionary<string, object> Properties { get;}
        public ItemDefinition(string id, IDictionary<string, object> properties)
        {
            this.Id = id;
            this.Properties = properties;
        }
    }
}