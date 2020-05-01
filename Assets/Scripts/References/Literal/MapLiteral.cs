using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class MapLiteral : MapContainer
    {
        private readonly Dictionary<string, ValueContainer> properties;

        public MapLiteral(Dictionary<string, ValueContainer> properties)
        {
            this.properties = properties != null ? properties : new Dictionary<string, ValueContainer>();
        }

        public Dictionary<string, ValueContainer> Get(IdleEngine engine)
        {
            return properties;
        }

        public IEnumerator<KeyValuePair<string, ValueContainer>> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public void Set(string propertyName, ValueContainer value)
        {
            properties[propertyName] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return properties.GetEnumerator();
        }
    }
}