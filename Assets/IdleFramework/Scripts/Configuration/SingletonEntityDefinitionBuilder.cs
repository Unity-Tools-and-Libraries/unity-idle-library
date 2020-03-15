using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    /*
     * Builder for a Singleton Entity.
     * 
     * A singleton entity is a category of entity where each instance can have different properties.
     */
    public class SingletonEntityDefinitionBuilder : SingletonEntityProperties, Builder<SingletonEntityDefinition>
    {
        private readonly string singletonKey;
        private readonly ISet<SingletonEntityInstanceConfigurationBuilder> instanceConfigurations = new HashSet<SingletonEntityInstanceConfigurationBuilder>();
        private readonly ISet<string> definedSingletonProperties = new HashSet<string>();
        public SingletonEntityDefinitionBuilder(string key)
        {
            this.singletonKey = key;
        }

        public SingletonEntityDefinitionBuilder CanHaveProperty(string property)
        {
            definedSingletonProperties.Add(property);
            return this;
        }

        public SingletonEntityInstanceConfigurationBuilder WithInstance(string singletonInstanceKey)
        {
            return new SingletonEntityInstanceConfigurationBuilder(singletonInstanceKey, this);
        }

        public string SingletonTypeKey => singletonKey;

        public SingletonEntityDefinition Build()
        {
            return new SingletonEntityDefinition(singletonKey, definedSingletonProperties, instanceConfigurations);
        }

        public class SingletonEntityInstanceConfigurationBuilder : Builder<SingletonEntityDefinition>
        {
            private SingletonEntityDefinitionBuilder parent;
            private readonly string instanceKey;
            private readonly Dictionary<string, BigDouble> instanceProperties = new Dictionary<string, BigDouble>();

            public string InstanceKey => instanceKey;
            public Dictionary<string, BigDouble> InstanceProperties { get => instanceProperties; }
            public string SingletonTypeKey => parent.SingletonTypeKey;

            public SingletonEntityInstanceConfigurationBuilder(string singletonInstanceKey, SingletonEntityDefinitionBuilder parent)
            {
                this.instanceKey = singletonInstanceKey;
                this.parent = parent;
                this.parent.instanceConfigurations.Add(this);
            }

            public SingletonEntityDefinitionBuilder And()
            {
                return parent;
            }

            public SingletonEntityDefinition Build()
            {
                return parent.Build();
            }

            public SingletonEntityInstanceConfigurationBuilder WithProperty(string propertyName, BigDouble propertyValue)
            {
                instanceProperties.Add(propertyName, propertyValue);
                return this;
            }
        }
    }
}