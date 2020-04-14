using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public interface MapContainer : ValueContainer, IEnumerable<KeyValuePair<string, ValueContainer>>
    {
        Dictionary<string, ValueContainer> Get(IdleEngine engine);
        void Set(string propertyName, ValueContainer value);
    }

    public static class MapContainerExtensions
    {
        public static MapContainer AsMap(this ValueContainer value)
        {
            if(typeof(MapContainer).IsAssignableFrom(value.GetType())) {
                return (MapContainer)value;
            }
            return Literal.Of((Dictionary<string, ValueContainer>)null);
        }
    }
}