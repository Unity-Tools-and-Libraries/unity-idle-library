
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions
{
    public class ResourceDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public IDictionary<string, object> Properties { get; set; }

        public ResourceDefinition(string Id, string Name, IDictionary<string, object> properties = null)
        {
            this.Id = Id;
            this.Name = Name;
            this.Properties = properties != null ? properties : new Dictionary<string, object>();
        }
    }
}