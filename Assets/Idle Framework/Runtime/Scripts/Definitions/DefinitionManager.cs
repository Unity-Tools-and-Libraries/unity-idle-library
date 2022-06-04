using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Definitions
{
    public class DefinitionManager : IDefinitionManager
    {
        private Dictionary<string, IDictionary<string, IDefinition>> definitions;

        public T GetDefinition<T>(string typeName, string id) where T : IDefinition
        {
            IDictionary<string, IDefinition> definitions;
            IDefinition definition = null;
            if (this.definitions.TryGetValue(typeName, out definitions))
            {
                definitions.TryGetValue(id, out definition);
            }
            return (T)definition;
        }

        public ICollection<T> GetDefinitions<T>(string typeName) where T : IDefinition
        {
            IDictionary<string, IDefinition> definitions;
            this.definitions.TryGetValue(typeName, out definitions);
            return (ICollection<T>)definitions;
        }

        public void SetDefinitions(string typeName, IDictionary<string, IDefinition> definitions)
        {
            this.definitions[typeName] = definitions;
        }
    }
}