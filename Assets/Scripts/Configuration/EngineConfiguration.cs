using BreakInfinity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace io.github.thisisnozaku.idle.framework.Configuration
{
    public class EngineConfiguration
    {
        public static readonly List<string> RESERVED_NAMES = new List<string> { "resources" };
        private IDictionary<string, ValueContainerDefinition> globalProperties = new Dictionary<string, ValueContainerDefinition>();
        private IDictionary<string, ResourceDefinition> resources = new Dictionary<string, ResourceDefinition>();
        public IDictionary<string, ValueContainerDefinition> GlobalProperties
        {
            get => new ReadOnlyDictionary<string, ValueContainerDefinition>(globalProperties);
        }

        public IDictionary<string, ResourceDefinition> Resources
        {
            get => new ReadOnlyDictionary<string, ResourceDefinition>(resources);
        }

        public void DeclareGlobalProperty(string propertyName)
        {
            DeclareGlobalProperty(propertyName, new ValueContainerDefinitionBuilder()
                .Build());
        }

        public void DeclareGlobalProperty(string property, ValueContainerDefinition valueReferenceDefinition)
        {
            if (RESERVED_NAMES.Contains(property))
            {
                throw new InvalidOperationException(string.Format("Cannot declare a property with reserved name '{0}'", property));
            }
            globalProperties[property] = valueReferenceDefinition;
        }

        public void DeclareGlobalProperty(string propertyName, BigDouble value)
        {
            DeclareGlobalProperty(propertyName, new ValueContainerDefinitionBuilder().WithStartingValue(value).Build());
        }

        public void DeclareResource(string resourceName)
        {
            resources[resourceName] = new ResourceDefinition(resourceName, BigDouble.Zero);
        }

        public void DeclareResource(ResourceDefinitionBuilder resourceDefinitionBuilder)
        {
            resources[resourceDefinitionBuilder.Id] = resourceDefinitionBuilder.Build();
        }
    }
}