using BreakInfinity;
using IdleFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IdleFramework
{
    public class PropertyHolder : IPropertiesHolder, CanSnapshot<PropertyHolderSnapshot>
    {
        private readonly Dictionary<string, NumberContainer> numberProperties = new Dictionary<string, NumberContainer>();
        private readonly Dictionary<string, StringContainer> stringProperties = new Dictionary<string, StringContainer>();
        private readonly Dictionary<string, BooleanContainer> booleanProperties = new Dictionary<string, BooleanContainer>();
        private readonly Dictionary<string, MapContainer> mapProperties = new Dictionary<string, MapContainer>();
        private readonly Dictionary<string, ListContainer> listProperties = new Dictionary<string, ListContainer>();

        public void Set(string property, StringContainer value)
        {
            stringProperties[property] = value;
        }
        public void Set(string property, NumberContainer value)
        {
            numberProperties[property] = value;
        }
        public void Set(string property, BooleanContainer value)
        {
            booleanProperties[property] = value;
        }
        public void Set(string property, MapContainer value)
        {
            mapProperties[property] = value;
        }
        public void Set(string property, ListContainer value)
        {
            listProperties[property] = value;
        }

        public BooleanContainer GetBoolean(string property)
        {
            return booleanProperties[property];
        }
        public StringContainer GetString(string property)
        {
            return stringProperties[property];
        }
        public NumberContainer GetNumber(string property)
        {
            if(!ContainsNumberProperty(property))
            {
                throw new WrongPropertyTypeException(property, "number", TypeOfProperty(property));
            }
            return numberProperties[property];
        }
        public ListContainer GetList(string property)
        {
            if(!ContainsListProperty(property))
            {
                throw new WrongPropertyTypeException(property, "list", TypeOfProperty(property));
            }
            return listProperties[property];
        }

        public MapContainer GetMap(string property)
        {
            if(!ContainsMapProperty(property))
            {
                throw new WrongPropertyTypeException(property, "map", TypeOfProperty(property));
            }
            return mapProperties[property];
        }

        public void AddAll(Dictionary<string, NumberContainer> properties)
        {
            foreach (var entry in properties)
            {
                Set(entry.Key, entry.Value);
            }
        }
        public void AddAll(Dictionary<string, StringContainer> properties)
        {
            foreach (var entry in properties)
            {
                Set(entry.Key, entry.Value);
            }
        }
        public void AddAll(Dictionary<string, BooleanContainer> properties)
        {
            foreach (var entry in properties)
            {
                Set(entry.Key, entry.Value);
            }
        }

        public IDictionary<string, StringContainer> StringProperties => new ReadOnlyDictionary<string, StringContainer>(stringProperties);
        public IDictionary<string, NumberContainer> NumberProperties => new ReadOnlyDictionary<string, NumberContainer>(numberProperties);
        public IDictionary<string, BooleanContainer> BooleanProperties => new ReadOnlyDictionary<string, BooleanContainer>(booleanProperties);
        public IDictionary<string, MapContainer> MapProperties => new ReadOnlyDictionary<string, MapContainer>(mapProperties);
        public IDictionary<string, ListContainer> ListProperties => new ReadOnlyDictionary<string, ListContainer>(listProperties);

        public bool ContainsProperty(string propertyName)
        {
            return ContainsStringProperty(propertyName) || ContainsBooleanProperty(propertyName) || ContainsNumberProperty(propertyName) || ContainsMapProperty(propertyName) || ContainsListProperty(propertyName);
        }

        public bool ContainsStringProperty(string propertyName)
        {
            return stringProperties.ContainsKey(propertyName);
        }

        public bool ContainsBooleanProperty(string propertyName)
        {
            return booleanProperties.ContainsKey(propertyName);
        }

        public bool ContainsNumberProperty(string propertyName)
        {
            return numberProperties.ContainsKey(propertyName);
        }
        public bool ContainsMapProperty(string propertyName)
        {
            return mapProperties.ContainsKey(propertyName);
        }

        public bool ContainsListProperty(string propertyName)
        {
            return ListProperties.ContainsKey(propertyName);
        }

        public void AddAll(Dictionary<string, MapContainer> properties)
        {
            throw new NotImplementedException();
        }

        private string TypeOfProperty(string propertyName)
        {
            if(ContainsStringProperty(propertyName))
            {
                return "string";
            }
            if(ContainsNumberProperty(propertyName))
            {
                return "number";
            }
            if(ContainsBooleanProperty(propertyName))
            {
                return "boolean";
            }
            return "unknown";
        }

        public PropertyHolderSnapshot GetSnapshot(IdleEngine engine)
        {
            return new PropertyHolderSnapshot(
                numberProperties.Select(np => new KeyValuePair<string, BigDouble>(np.Key, np.Value.Get(engine))), 
                stringProperties.Select(np => new KeyValuePair<string, string>(np.Key, np.Value.Get(engine))),
                booleanProperties.Select(np => new KeyValuePair<string, bool>(np.Key, np.Value.Get(engine)))
                );
        }

        public void LoadFromSnapshot(PropertyHolderSnapshot snapshot)
        {
            foreach(var stringProperty in snapshot.StringProperties)
            {
                if(stringProperties[stringProperty.Key] is StringLiteral)
                {
                    stringProperties[stringProperty.Key] = Literal.Of(stringProperty.Value);
                }
            }
            foreach (var numberProperty in snapshot.NumberProperties)
            {
                if (numberProperties[numberProperty.Key] is NumberLiteral)
                {
                    numberProperties[numberProperty.Key] = Literal.Of(numberProperty.Value);
                }
            }
            foreach (var booleanProperty in snapshot.BooleanProperties)
            {
                if (booleanProperties[booleanProperty.Key] is StringLiteral)
                {
                    booleanProperties[booleanProperty.Key] = Literal.Of(booleanProperty.Value);
                }
            }
        }
    }
}