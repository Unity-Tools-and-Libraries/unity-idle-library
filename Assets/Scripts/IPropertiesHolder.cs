using System.Collections.Generic;

namespace IdleFramework
{
    public interface IPropertiesHolder
    {
        IDictionary<string, BooleanContainer> BooleanProperties { get; }
        IDictionary<string, NumberContainer> NumberProperties { get; }
        IDictionary<string, StringContainer> StringProperties { get; }
        IDictionary<string, MapContainer> MapProperties { get; }

        void AddAll(Dictionary<string, BooleanContainer> properties);
        void AddAll(Dictionary<string, NumberContainer> properties);
        void AddAll(Dictionary<string, StringContainer> properties);
        void AddAll(Dictionary<string, MapContainer> properties);
        
        bool ContainsProperty(string propertyName);
        
        BooleanContainer GetBoolean(string property);
        NumberContainer GetNumber(string property);
        StringContainer GetString(string property);
        MapContainer GetMap(string property);
        
        void Set(string property, BooleanContainer value);
        void Set(string property, MapContainer value);
        void Set(string property, NumberContainer value);
        void Set(string property, StringContainer value);
    }
}