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
        private readonly ISet<SingletonEntityInstanceBuilder> instanceConfigurations = new HashSet<SingletonEntityInstanceBuilder>();
        private readonly ISet<string> definedSingletonProperties = new HashSet<string>();
        private readonly ISet<string> mandadorySingletonProperties = new HashSet<string>();
        public SingletonEntityDefinitionBuilder(string key)
        {
            this.singletonKey = key;
        }

        public SingletonEntityDefinitionBuilder CanHaveProperty(string property)
        {
            definedSingletonProperties.Add(property);
            return this;
        }

        public SingletonEntityDefinitionBuilder WithInstance(SingletonEntityInstanceBuilder instance)
        {;
            this.instanceConfigurations.Add(instance);
            return this;
        }

        public string SingletonTypeKey => singletonKey;

        public SingletonEntityDefinition Build()
        {
            return new SingletonEntityDefinition(singletonKey, definedSingletonProperties, instanceConfigurations);
        }

    }
}