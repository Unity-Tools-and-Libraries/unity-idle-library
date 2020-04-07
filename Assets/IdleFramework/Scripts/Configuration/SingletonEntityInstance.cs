using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IdleFramework
{
    public class SingletonEntityInstance
    {
        private readonly string instanceKey;
        private readonly PropertyContainer instanceProperties;
        private readonly SingletonEntityDefinition definition;

        public SingletonEntityInstance(string instanceKey, SingletonEntityDefinition definition, PropertyContainer instanceProperties)
        {
            this.instanceKey = instanceKey;
            this.definition = definition;
            this.instanceProperties = instanceProperties;
        }

        public string InstanceKey => instanceKey;

        public ValueContainer GetProperty(string propertyName)
        {
            definition.AssertHasProperty(propertyName);
            return instanceProperties.Get(propertyName);
        }
    }

    public class SingletonEntityInstanceBuilder : Builder<SingletonEntityInstance>
    {
        private readonly string instanceKey;
        private SingletonEntityDefinition definition;
        private Dictionary<string, ValueContainer> properties = new Dictionary<string, ValueContainer>();
        private Dictionary<string, PropertyContainer> containerProperties = new Dictionary<string, PropertyContainer>();

        public string InstanceKey => instanceKey;
        public string TypeKey => definition.SingletonTypeKey;

        internal SingletonEntityInstanceBuilder WithDefinition(SingletonEntityDefinition definition)
        {
            this.definition = definition;
            return this;
        }

        public SingletonEntityInstanceBuilder(string instanceKey)
        {
            this.instanceKey = instanceKey;
        }

        public SingletonEntityInstance Build()
        {
            if(definition == null)
            {
                throw new InvalidOperationException("Cannot build if definition is null");
            }
            return new SingletonEntityInstance(instanceKey, definition,
                new SimplePropertyContainer.SimplePropertyContainerBuilder()
                .WithProperties(properties)
                .Build());
        }

        public SingletonEntityInstanceBuilder WithProperties(Dictionary<string, ValueContainer> properties)
        {
            this.properties = properties;
            return this;
        }

        public static SingletonEntityInstanceBuilder For(string instanceKey)
        {
            return new SingletonEntityInstanceBuilder(instanceKey);
        }
    }
}