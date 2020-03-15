using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class SingletonEntityDefinition : SingletonEntityProperties
    {
        private readonly string singletonTypeKey;
        private readonly Dictionary<string, SingletonEntity> instances = new Dictionary<string, SingletonEntity>();

        public SingletonEntityDefinition(string singletonKey)
        {
            this.singletonTypeKey = singletonKey;
        }

        public string SingletonTypeKey => singletonTypeKey;

        public Dictionary<string, SingletonEntity> Instances => instances;

        public double GetInstanceByKey(string instanceKey)
        {
            throw new NotImplementedException();
        }
    }
}