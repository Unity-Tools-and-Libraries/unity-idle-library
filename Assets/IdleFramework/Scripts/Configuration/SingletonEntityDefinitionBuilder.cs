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
        public SingletonEntityDefinitionBuilder(string key)
        {
            this.singletonKey = key;
        }

        public string SingletonTypeKey => singletonKey;

        public SingletonEntityDefinition Build()
        {
            return new SingletonEntityDefinition(singletonKey);
        }
    }
}