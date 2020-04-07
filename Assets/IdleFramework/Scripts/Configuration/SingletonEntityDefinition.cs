using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class SingletonEntityDefinition : SingletonEntityProperties
    {
        private readonly string singletonTypeKey;
        private readonly ISet<string> definedProperties;
        private readonly Dictionary<string, SingletonEntityInstance> instances = new Dictionary<string, SingletonEntityInstance>();

        public SingletonEntityDefinition(string singletonKey, ISet<string> definedProperties, ISet<SingletonEntityInstanceBuilder> instanceConfigurations)
        {
            this.singletonTypeKey = singletonKey;
            this.definedProperties = definedProperties;
            foreach(var instance in instanceConfigurations)
            {
                instance.WithDefinition(this);
                if (instances.ContainsKey(instance.InstanceKey)){
                    throw new InvalidOperationException(String.Format("Multiple singletons of the type {0} had the instance key of {1}", instance.TypeKey, instance.InstanceKey));
                }
                instances.Add(instance.InstanceKey, instance.Build());
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

        public void AssertHasProperty(string propertyName)
        {
            if(!DefinesProperty(propertyName))
            {
                throw new InvalidOperationException(string.Format("Does not define property {0}", propertyName));
            }
        }
    }
}