using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class SingletonEntityInstance
    {
        private readonly Dictionary<string, BigDouble> instanceProperties = new Dictionary<string, BigDouble>();
        private readonly SingletonEntityDefinition definition;

        public SingletonEntityInstance(SingletonEntityDefinition definition, Dictionary<string, BigDouble> instanceProperties)
        {
            this.definition = definition;
            this.instanceProperties = instanceProperties;
        }

        public BigDouble GetPropertyValue(string propertyName)
        {
            BigDouble value = 0;
            instanceProperties.TryGetValue(propertyName, out value);
            return value;
        }
    }
}