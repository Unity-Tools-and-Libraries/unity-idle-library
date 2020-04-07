using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class SimplePropertyContainer : PropertyContainer
    {
        private readonly Dictionary<string, ValueContainer> properties = new Dictionary<string, ValueContainer>();

        public SimplePropertyContainer() : this(null) { }

        private SimplePropertyContainer(Dictionary<string, ValueContainer> properties)
        {
            if(properties != null)
            {
                this.properties = properties;
            }
        }

        public static PropertyContainer EMPTY = new SimplePropertyContainer();

        public IEnumerable<KeyValuePair<string, ValueContainer>> Properties => properties;

        public ValueContainer Get(string propertyName)
        {
            ValueContainer value;
            properties.TryGetValue(propertyName, out value);
            return value;
        }

        public void SetProperty(string propertyName, ValueContainer value)
        {
            throw new NotImplementedException();
        }

        public class SimplePropertyContainerBuilder : Builder<SimplePropertyContainer>
        {
            private Dictionary<string, ValueContainer> properties;

            public SimplePropertyContainerBuilder WithProperties(Dictionary<string, ValueContainer> properties)
            {
                this.properties = properties;
                return this;
            }

            public SimplePropertyContainer Build()
            {
                return new SimplePropertyContainer(properties);
            }
        }

    }
}