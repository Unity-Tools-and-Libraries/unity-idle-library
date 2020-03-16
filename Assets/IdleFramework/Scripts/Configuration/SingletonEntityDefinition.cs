using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class SingletonEntityDefinition : SingletonEntityProperties
    {
        private readonly string singletonTypeKey;
        private readonly ISet<string> definedProperties;
        private readonly Dictionary<string, SingletonEntityInstance> instances = new Dictionary<string, SingletonEntityInstance>();

        public SingletonEntityDefinition(string singletonKey, ISet<string> definedProperties, ISet<SingletonEntityDefinitionBuilder.SingletonEntityInstanceConfigurationBuilder> instanceConfigurations)
        {
            this.singletonTypeKey = singletonKey;
            this.definedProperties = definedProperties;
            foreach(var instanceBuilder in instanceConfigurations)
            {
                if (instances.ContainsKey(instanceBuilder.InstanceKey)){
                    throw new InvalidOperationException(String.Format("Multiple singletons of the type {0} had the instance key of {1}", instanceBuilder.SingletonTypeKey, instanceBuilder.InstanceKey));
                }
                instances.Add(instanceBuilder.InstanceKey, new SingletonEntityInstance(this, instanceBuilder.InstanceProperties));
            }
        }

        public string SingletonTypeKey => singletonTypeKey;

        public bool DefinesProperty(string propertyName)
        {
            return definedProperties.Contains(propertyName);
        }

        public Dictionary<string, SingletonEntityInstance> Instances => instances;

        public SingletonEntityInstance GetInstanceByKey(string instanceKey)
        {
            SingletonEntityInstance singleton = null;
            instances.TryGetValue(instanceKey, out singleton);
            return singleton;
        }
    }
}